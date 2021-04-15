using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using LDZ.Coinbase.Api.Hosting;
using LDZ.Coinbase.Api.Json.Serialization;
using LDZ.Coinbase.Api.Net.Http;
using LDZ.Coinbase.Api.Net.Http.Headers;
using LDZ.Coinbase.Api.Net.WebSockets;
using LDZ.Coinbase.Api.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace LDZ.Coinbase.Api.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCoinbaseProApi(this IServiceCollection services, Action<ICoinbaseApiBuilder>? configure = null, Action<OptionsBuilder<CoinbaseApiOptions>>? configureOptions = null)
        {
            var builder = services.AddCoinbaseProApi(configureOptions ?? (b => b.UseProduction()));
            configure?.Invoke(builder);

            return services;
        }

        public static ICoinbaseApiBuilder AddCoinbaseProApi(this IServiceCollection services, Action<OptionsBuilder<CoinbaseApiOptions>> configureOptions)
        {
            services
                .ConfigureCoinbaseSerializerOptions();

            services
                .AddTransient<IMarketDataClient, MarketDataClient>()
                .AddTransient<ITradingClient, TradingClient>()
                .AddTransient<MessageAuthenticationCodeHandler>()
                .AddSingleton<ThrottlingPolicy>()
                .AddTransient<ThrottlingPolicyHandler>()
                .AddScoped<IWebSocketFeed, WebSocketFeed>();

            services
                .TryAddTransient<IClientWebSocketFacade, ClientWebSocketFacade>();

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

            var options = services.AddOptions<CoinbaseApiOptions>();
            configureOptions(options);

            return new CoinbaseApiBuilder(services);
        }

        private static IServiceCollection ConfigureCoinbaseSerializerOptions(this IServiceCollection services) =>
            services.Configure<JsonSerializerOptions>(options =>
            {
                options.Converters.Add(new AggregatedOrderJsonConverter());
                options.Converters.Add(new DecimalConverter());
                options.Converters.Add(new OrderSideConverter());
                options.Converters.Add(new OrderTypeConverter());
                options.Converters.Add(new TradeSideConverter());

                options.Converters.Add(new FeedChannelConverter());
                options.Converters.Add(new FeedRequestMessageConverter());
                options.Converters.Add(new FeedResponseMessageConverter());

                options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });
    }
}
