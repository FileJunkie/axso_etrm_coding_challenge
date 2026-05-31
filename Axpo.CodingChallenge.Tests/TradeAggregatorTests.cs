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
            1,
            hour: 5,
            minute: 0,
            0,
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
    
    private static IEnumerable<PowerTrade> GetInputs(DateTime date)
    {
        var trade1 = PowerTrade.Create(date, 24);
        foreach (var period in trade1.Periods)
        {
            period.SetVolume(100);
        }

        yield return trade1;

        var trade2 =  PowerTrade.Create(date, 24);
        foreach (var period in trade2.Periods)
        {
            period.SetVolume(period.Period < 12 ? 50 : -20);
        }
    }
   
    private static IEnumerable<AggregatedTrade> GetExpected()
    {
        for (var time = new TimeOnly(23, 0); time.Hour is 23 or < 10; time = time.AddHours(1))
        {
            yield return new(time, 100);
        }

        for (var time = new TimeOnly(10, 0); time.Hour < 23; time = time.AddHours(1))
        {
            yield return new(time, 80);
        }
    }
}