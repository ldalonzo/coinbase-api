using System;
using LDZ.Coinbase.Api.Net.Http;
using LDZ.Coinbase.Api.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LDZ.Coinbase.Api.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMarketDataClient(this IServiceCollection services, Action<OptionsBuilder<CoinbaseClientOptions>> configure)
        {
            services
                .AddTransient<IMarketDataClient, MarketDataClient>()
                .AddSingleton<ThrottlingPolicy>()
                .AddTransient<ThrottlingPolicyHandler>();

            configure(services.AddOptions<CoinbaseClientOptions>());

            services
                .AddHttpClient(ClientNames.MarketData, (provider, client) =>
                {
                    client.BaseAddress = provider.GetRequiredService<IOptions<CoinbaseClientOptions>>().Value.BaseAddress;
                    client.DefaultRequestHeaders.Add("User-Agent", "CoinbaseProClient");
                })
                .AddHttpMessageHandler<ThrottlingPolicyHandler>();

            return services;
        }

        public static IServiceCollection AddMarketDataClient(this IServiceCollection services) => services
            .AddMarketDataClient(builder => builder.Configure(o =>
            {
                o.BaseAddress = new Uri("https://api.pro.coinbase.com", UriKind.Absolute);
            }));
    }
}
