using Axpo.CodingChallenge.Models;

namespace Axpo.CodingChallenge.Services;

public interface ITradeAggregator
{
    Task<IEnumerable<AggregatedTrade>> AggregateTradesAsync(DateTime date);
}