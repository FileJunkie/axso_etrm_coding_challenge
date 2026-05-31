using Axpo.CodingChallenge.Models;

namespace Axpo.CodingChallenge.Services;

public interface IDataWriter
{
    Task WriteDataAsync(IEnumerable<AggregatedTrade> data);
}