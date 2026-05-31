using Axpo.CodingChallenge.Models;
using Axpo.CodingChallenge.Utils;

namespace Axpo.CodingChallenge.Services;

public class TradeAggregator(
    IPowerService powerService,
    ILogger<TradeAggregator> logger) : ITradeAggregator
{
    // It's a list of the same size on every call,
    // Where every entry is updated on every new PowerTrade object received
    // Therefore it's a Task<IEnumerable>, not IAsyncEnumerable
    public async Task<IEnumerable<AggregatedTrade>> AggregateTradesAsync(DateTime date)
    {
        // this is a mapping of the period number to the amount of trades per period
        // in general, there sure should be 24 periods, but daylight savings time exists,
        // and it's not documented if GetTradesAsync takes it into account or not
        // current implementation of PowerService also generates mock data even if it would be in the future
        // which wouldn't be so in the real life
        var aggregatedTrades = new Dictionary<int, double>();

        var trades = await powerService.GetTradesAsync(date);
        foreach (var trade in trades)
        {
            logger.LogTrace("Got trade {TradeId}", trade.TradeId);
            foreach (var period in trade.Periods)
            {
                logger.LogTrace("Got Period {Period}: volume {Volume}", period.Period, period.Volume);
                // periods are documented to be 1-based, not 0-based
                var aggregatedVolume = aggregatedTrades.GetValueOrDefault(period.Period - 1, 0);
                logger.LogTrace("Aggregated volume is {Volume}", aggregatedVolume);
                aggregatedVolume += period.Volume;
                aggregatedTrades[period.Period - 1] = aggregatedVolume;
                logger.LogTrace("Updated volume is {Volume}", aggregatedVolume);
            }
        }

        // Okay, we aggregated the data by period numbers.
        // It's assumed that in case of an hour skipped because of DST,
        // period number is _not_ skipped, on the other words,
        // period number is considered to be an offset to UTC time, not wall time
        // and even if a period number _is_ skipped, it's just the data that was missing
        var minPeriodNumber = aggregatedTrades.Keys.Min();
        var maxPeriodNumber = aggregatedTrades.Keys.Max();
        var firstHourOfDay = GetFirstHourOfDay(date);
        var result = new List<AggregatedTrade>();
        for (var i = minPeriodNumber; i <= maxPeriodNumber; i++)
        {
            if (aggregatedTrades.TryGetValue(i, out var volume))
            {
                var time = TimeOnly.FromDateTime(firstHourOfDay.AddHours(i).DateTime);
                result.Add(new AggregatedTrade (time, volume));
            }
        }

        return result;
    }

    private static DateTimeOffset GetFirstHourOfDay(DateTime date)
    {
        // Creating the date object for the date in question, midnight and local TZ
        var dateInWallTime = new DateTimeOffset(
            date.Year,
            date.Month,
            date.Day,
            0, 0, 0,
            TimeUtils.LocalTimeZoneInfo.GetUtcOffset(date));

        // Going to 23:00 of the previous day
        return dateInWallTime.AddHours(-1);
    }
}