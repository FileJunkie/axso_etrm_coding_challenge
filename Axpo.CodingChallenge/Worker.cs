using Axpo.CodingChallenge.Services;

namespace Axpo.CodingChallenge;

public class Worker(
    ITradeAggregator tradeAggregator,
    IDataWriter dataWriter,
    ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Started at: {Time}", DateTimeOffset.Now);
        var data = await tradeAggregator.AggregateTradesAsync(DateTime.Now);
        logger.LogInformation("Data aggregation done at: {Time}", DateTimeOffset.Now);
        await dataWriter.WriteDataAsync(data);
        logger.LogInformation("Writing data done at: {Time}", DateTimeOffset.Now);
    }
}
