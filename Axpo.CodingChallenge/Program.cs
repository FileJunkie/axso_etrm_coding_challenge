using Axpo;
using Axpo.CodingChallenge;
using Axpo.CodingChallenge.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services
    .AddSingleton<IPowerService, PowerService>()
    .AddSingleton<ITradeAggregator, TradeAggregator>()
    .AddSingleton<IDataWriter, DataWriter>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
await host.RunAsync();
