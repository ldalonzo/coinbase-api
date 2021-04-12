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
    public class CoinbaseApiFactory
    {
        public static CoinbaseApiFactory Create(Action<ICoinbaseApiBuilder>? configure = null, Action<OptionsBuilder<CoinbaseApiOptions>>? configureOptions = null)
        {
            var services = new ServiceCollection();

            var builder = services
                .AddCoinbaseProApi(configureOptions ?? (b => b.UseProduction()));

            configure?.Invoke(builder);

            return new CoinbaseApiFactory(services.BuildServiceProvider());
        }

        private CoinbaseApiFactory(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        private IServiceProvider ServiceProvider { get; }

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
    }
}
