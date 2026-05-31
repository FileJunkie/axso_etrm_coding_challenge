using Axpo.CodingChallenge;
using Axpo.CodingChallenge.Configuration;
using Axpo.CodingChallenge.Services;
using Microsoft.Extensions.Options;
using Quartz;

var builder = Host.CreateApplicationBuilder(args);
builder.Services
    .AddServices()
    .AddQuartz()
    .AddQuartzHostedService();

builder.Services.AddAndConfigureOptions<SchedulerOptions>(builder.Configuration);
builder.Services.AddAndConfigureOptions<WriterOptions>(builder.Configuration);

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
