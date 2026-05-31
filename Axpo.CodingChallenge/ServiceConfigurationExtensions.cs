using Axpo.CodingChallenge.Services;
using Axpo.CodingChallenge.Utils;

namespace Axpo.CodingChallenge;

public static class ServiceConfigurationExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddServices() => services
            .AddSingleton<IPowerService, PowerService>()
            .AddSingleton<ITradeAggregator, TradeAggregator>()
            .AddSingleton<IDataWriter, DataWriter>()
            .AddSingleton<Retrier>();

        public IServiceCollection AddAndConfigureOptions<T>(IConfiguration configuration) where T : class => services
            .AddOptions<T>()
            .Bind(configuration.GetSection(typeof(T).Name))
            .ValidateDataAnnotations()
            .ValidateOnStart()
            .Services;
    }
}