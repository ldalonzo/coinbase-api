using System;
using System.Threading;
using System.Threading.Tasks;
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

        public async Task<IMarketDataFeedMessagePublisher> StartMarketDataFeed(CancellationToken cancellationToken = default)
        {
            var marketDataFeed = ServiceProvider.CreateScope().ServiceProvider.GetRequiredService<IMarketDataFeedMessagePublisher>();
            await marketDataFeed.StartAsync(cancellationToken);

            return marketDataFeed;
        }

        public void Dispose()
        {
            ServiceProvider.Dispose();
        }
    }
}
