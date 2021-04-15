using System;
using LDZ.Coinbase.Api.DependencyInjection;
using LDZ.Coinbase.Api.Hosting;
using LDZ.Coinbase.Api.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LDZ.Coinbase.Api
{
    public class CoinbaseApiFactory : IDisposable
    {
        public static CoinbaseApiFactory Create(Action<ICoinbaseApiBuilder>? configure = null, Action<OptionsBuilder<CoinbaseApiOptions>>? configureOptions = null)
        {
            var services = new ServiceCollection();

            return new CoinbaseApiFactory(services
                .AddCoinbaseProApi(configure, configureOptions ?? (b => b.UseProduction()))
                .BuildServiceProvider()
            );
        }

        private CoinbaseApiFactory(ServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        private ServiceProvider ServiceProvider { get; }

        public IMarketDataClient CreateMarketDataClient()
            => ServiceProvider.CreateScope().ServiceProvider.GetRequiredService<IMarketDataClient>();

        public ITradingClient CreateTradingClient()
            => ServiceProvider.CreateScope().ServiceProvider.GetRequiredService<ITradingClient>();

        public IWebSocketFeed CreateWebSocketFeed()
            => ServiceProvider.CreateScope().ServiceProvider.GetRequiredService<IWebSocketFeed>();

        public void Dispose()
        {
            ServiceProvider.Dispose();
        }
    }
}
