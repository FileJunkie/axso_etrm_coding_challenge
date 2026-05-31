using Axpo;
using Axpo.CodingChallenge.Configuration;
using Axpo.CodingChallenge.Services;
using Axpo.CodingChallenge.Utils;
using Microsoft.Extensions.Options;
using Quartz;

var builder = Host.CreateApplicationBuilder(args);
builder.Services
    .AddSingleton<IPowerService, PowerService>()
    .AddSingleton<ITradeAggregator, TradeAggregator>()
    .AddSingleton<IDataWriter, DataWriter>()
    .AddSingleton<Retrier>()
    .AddQuartz()
    .AddQuartzHostedService();

builder.Services
    .AddOptions<SchedulerOptions>()
    .Bind(builder.Configuration.GetSection(nameof(SchedulerOptions)))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services
    .AddOptions<WriterOptions>()
    .Bind(builder.Configuration.GetSection(nameof(WriterOptions)))
    .ValidateDataAnnotations()
    .ValidateOnStart();

var host = builder.Build();

var schedulerOptions = host.Services.GetRequiredService<IOptions<SchedulerOptions>>().Value;

var job = JobBuilder.Create<AggregationJob>().Build();
var trigger = TriggerBuilder.Create()
    .StartNow()
    .WithSimpleSchedule(x => x
        .WithIntervalInMinutes(schedulerOptions.IntervalInMinutes)
        .RepeatForever())
    .Build();

var schedulerFactory = host.Services.GetRequiredService<ISchedulerFactory>();
var scheduler = await schedulerFactory.GetScheduler();
await scheduler.ScheduleJob(job, trigger);

await host.RunAsync();
