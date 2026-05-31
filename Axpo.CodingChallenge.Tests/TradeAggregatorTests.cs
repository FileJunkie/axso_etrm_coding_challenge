using Axpo.CodingChallenge.Models;
using Axpo.CodingChallenge.Services;
using Axpo.CodingChallenge.Utils;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

namespace Axpo.CodingChallenge.Tests;

public class TradeAggregatorTests
{
    [Fact]
    public async Task TradeAggregator_ShouldReturnExpectedResults()
    {
        // Arrange
        var testDate = new DateTime(
            year: 2015,
            month: 4,
            day: 1,
            hour: 5,
            minute: 0,
            second: 0,
            kind: DateTimeKind.Utc);
        var powerServiceMock = Substitute.For<IPowerService>();
        powerServiceMock.GetTradesAsync(testDate)
            .Returns(GetInputs(testDate));

        var aggregator = new TradeAggregator(
            powerServiceMock,
            new Retrier(Substitute.For<ILogger<Retrier>>()),
            Substitute.For<ILogger<TradeAggregator>>());

        // Act
        var result =
            (await aggregator.AggregateTradesAsync(testDate)).ToList();

        // Assert
        result.ShouldBeEquivalentTo(GetExpected().ToList());
        await powerServiceMock.Received(1).GetTradesAsync(testDate);
    }

    [Fact]
    public async Task TradeAggregator_MustSurviveDst()
    {
        // Arrange
        var testDate = new DateTime(
            year: 2026,
            month: 3,
            day: 29,
            hour: 5,
            minute: 0,
            second: 0,
            kind: DateTimeKind.Utc);
        var powerServiceMock = Substitute.For<IPowerService>();
        powerServiceMock.GetTradesAsync(testDate)
            .Returns(GetDstInputs(testDate));

        var aggregator = new TradeAggregator(
            powerServiceMock,
            new Retrier(Substitute.For<ILogger<Retrier>>()),
            Substitute.For<ILogger<TradeAggregator>>());

        // Act
        var result =
            (await aggregator.AggregateTradesAsync(testDate)).ToList();

        // Assert
        result.ShouldBeEquivalentTo(GetDstExpected().ToList());
        await powerServiceMock.Received(1).GetTradesAsync(testDate);
    }
    
    private static IEnumerable<PowerTrade> GetInputs(DateTime date)
    {
        var trade1 = PowerTrade.Create(date, 24);
        for (var i = 0; i < 24; i++)
        {
            trade1.Periods[i].SetVolume(100.0);
        }

        yield return trade1;

        var trade2 =  PowerTrade.Create(date, 24);
        for (var i = 0; i < 24; i++)
        {
            trade2.Periods[i].SetVolume(trade2.Periods[i].Period < 12 ? 50.0 : -20.0);
        }

        yield return trade2;
    }
   
    private static IEnumerable<AggregatedTrade> GetExpected()
    {
        for (var time = new TimeOnly(23, 0); time.Hour is 23 or < 10; time = time.AddHours(1))
        {
            yield return new(time, 150);
        }

        for (var time = new TimeOnly(10, 0); time.Hour < 23; time = time.AddHours(1))
        {
            yield return new(time, 80);
        }
    }
    
    private static IEnumerable<PowerTrade> GetDstInputs(DateTime date)
    {
        var trade = PowerTrade.Create(date, 23);
        for (var i = 0; i < 23; i++)
        {
            trade.Periods[i].SetVolume(i);
        }

        yield return trade;
    }
   
    private static IEnumerable<AggregatedTrade> GetDstExpected()
    {
        yield return new(new TimeOnly(23, 0), 0);
        yield return new(new TimeOnly(0, 0), 1);

        for (var i = 2; i < 23; i++)
        {
            yield return new(new TimeOnly(i, 0), i);
        }
    }
}