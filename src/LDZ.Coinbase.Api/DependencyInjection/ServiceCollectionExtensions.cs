using System;
using LDZ.Coinbase.Api.Net;
using LDZ.Coinbase.Api.Net.Http;
using LDZ.Coinbase.Api.Net.Http.Headers;
using LDZ.Coinbase.Api.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LDZ.Coinbase.Api.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCoinbaseProRestApi(this IServiceCollection services,
            Action<OptionsBuilder<CoinbaseApiOptions>>? configure = null,
            Action<OptionsBuilder<CoinbaseApiKeyOptions>>? configureApiKey = null)
            => AddCoinbaseProRestApiInternal(services, configure ?? (b => b.UseProduction()), configureApiKey);

        private static IServiceCollection AddCoinbaseProRestApiInternal(this IServiceCollection services,
            Action<OptionsBuilder<CoinbaseApiOptions>> configure,
            Action<OptionsBuilder<CoinbaseApiKeyOptions>>? configureApiKey = null)
        {
            services
                .AddTransient<IMarketDataClient, MarketDataClient>()
                .AddTransient<ITradingClient, TradingClient>()
                .AddSingleton<ThrottlingPolicy>()
                .AddTransient<ThrottlingPolicyHandler>()
                .AddTransient<MessageAuthenticationCodeHandler>();

            configure(services.AddOptions<CoinbaseApiOptions>());

            configureApiKey?.Invoke(services.AddOptions<CoinbaseApiKeyOptions>());

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
    }
}
