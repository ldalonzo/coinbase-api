using System;
using LDZ.Coinbase.Api.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LDZ.Coinbase.Api.Hosting
{
    internal class CoinbaseApiBuilder : ICoinbaseApiBuilder
    {
        public CoinbaseApiBuilder(IServiceCollection services)
        {
            Services = services;
        }

        private IServiceCollection Services { get; }

        public ICoinbaseApiBuilder ConfigureApiKey(Action<OptionsBuilder<CoinbaseApiKeyOptions>> configureApiKey)
        {
            var builder = Services.AddOptions<CoinbaseApiKeyOptions>();
            configureApiKey(builder);

            return this;
        }

        public ICoinbaseApiBuilder ConfigureFeed(Action<IWebSocketSubscriptionsBuilder> configureFeed)
        {
            var builder = new WebSocketSubscriptionsBuilder();
            configureFeed(builder);

            Services.AddOptions<MarketDataFeedMessagePublisherOptions>().PostConfigure(options =>
            {
                options.Handlers = builder.BuildHandlers();
                options.Subscriptions = builder.BuildSubscriptions();
            });

            return this;
        }
    }
}
