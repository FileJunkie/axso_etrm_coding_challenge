using Quartz;

namespace Axpo.CodingChallenge.Services;

public class AggregationJob(
    ITradeAggregator tradeAggregator,
    IDataWriter dataWriter,
    ILogger<AggregationJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("Started at: {Time}", DateTimeOffset.Now);
        var data = await tradeAggregator.AggregateTradesAsync(DateTime.Now);
        logger.LogInformation("Data aggregation done at: {Time}", DateTimeOffset.Now);
        await dataWriter.WriteDataAsync(data);
        logger.LogInformation("Writing data done at: {Time}", DateTimeOffset.Now);
    }
}