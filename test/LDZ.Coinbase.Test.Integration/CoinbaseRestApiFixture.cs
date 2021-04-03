using System.Threading.Tasks;
using LDZ.Coinbase.Api;
using LDZ.Coinbase.Api.DependencyInjection;
using LDZ.Coinbase.Api.Options;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LDZ.Coinbase.Test.Integration
{
    public class CoinbaseRestApiFixture : IAsyncLifetime
    {
        public IMarketDataClient MarketDataClient { get; private set; }

        public ITradingClient TradingClient { get; private set; }

        private ServiceProvider ServiceProvider { get; set; }

        public Task InitializeAsync()
        {
            var services = new ServiceCollection();

            ServiceProvider = services
                .AddCoinbaseProRestApi(_ => {}, api => api.UseSandbox())
                .BuildServiceProvider();

            MarketDataClient = ServiceProvider.GetRequiredService<IMarketDataClient>();
            TradingClient = ServiceProvider.GetRequiredService<ITradingClient>();

            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            await ServiceProvider.DisposeAsync();
        }
    }
}
