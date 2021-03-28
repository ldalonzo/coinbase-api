using System;
using LDZ.Coinbase.Api.Net.Http;
using LDZ.Coinbase.Api.Net.Http.Headers;
using LDZ.Coinbase.Api.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LDZ.Coinbase.Api.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCoinbaseProRestApi(this IServiceCollection services, Action<OptionsBuilder<CoinbaseApiOptions>> configure)
        {
            services
                .AddTransient<IMarketDataClient, MarketDataClient>()
                .AddTransient<ITradingClient, TradingClient>()
                .AddSingleton<ThrottlingPolicy>()
                .AddTransient<ThrottlingPolicyHandler>()
                .AddTransient<MessageAuthenticationCodeHandler>();

            var optionsBuilder = services.AddOptions<CoinbaseApiOptions>();
            configure(optionsBuilder);

            services
                .AddHttpClient(ClientNames.MarketData, (provider, client) =>
                {
                    client.BaseAddress = provider.GetRequiredService<IOptions<CoinbaseApiOptions>>().Value.RestApiBaseUri;
                    client.DefaultRequestHeaders.AddCoinbaseUserAgent();
                })
                .AddHttpMessageHandler<ThrottlingPolicyHandler>();

            services
                .AddHttpClient(ClientNames.TradingClient, (provider, client) =>
                {
                    client.BaseAddress = provider.GetRequiredService<IOptions<CoinbaseApiOptions>>().Value.RestApiBaseUri;
                    client.DefaultRequestHeaders.AddCoinbaseUserAgent();
                })
                .AddHttpMessageHandler<MessageAuthenticationCodeHandler>();

            return services;
        }

        public static IServiceCollection AddCoinbaseProRestApi(this IServiceCollection services) => services
            .AddCoinbaseProRestApi(builder => builder.Configure(o => { o.RestApiBaseUri = EndpointUriNames.RestApiUri; }));
    }
}
