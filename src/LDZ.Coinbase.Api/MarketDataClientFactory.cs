using System;
using LDZ.Coinbase.Api.DependencyInjection;
using LDZ.Coinbase.Api.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LDZ.Coinbase.Api
{
    public static class MarketDataClientFactory
    {
        public static IMarketDataClient CreateClient(Action<OptionsBuilder<CoinbaseApiOptions>>? configure = null) => new ServiceCollection()
            .AddCoinbaseProRestApi(configure)
            .BuildServiceProvider()
            .GetRequiredService<IMarketDataClient>();

        public static ITradingClient CreateTradingClient(Action<OptionsBuilder<CoinbaseApiKeyOptions>> configureApiKey, Action<OptionsBuilder<CoinbaseApiOptions>>? configure = null) => new ServiceCollection()
            .AddCoinbaseProRestApi(configure, configureApiKey)
            .BuildServiceProvider()
            .GetRequiredService<ITradingClient>();
    }
}
