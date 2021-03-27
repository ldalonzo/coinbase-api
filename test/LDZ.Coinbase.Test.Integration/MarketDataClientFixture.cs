using System;
using System.Threading.Tasks;
using LD.Coinbase.Api;
using LD.Coinbase.Api.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LDZ.Coinbase.Test.Integration
{
    public class MarketDataClientFixture : IAsyncLifetime
    {
        public IMarketDataClient MarketDataClient { get; private set; }

        private ServiceProvider ServiceProvider { get; set; }

        public Task InitializeAsync()
        {
            var services = new ServiceCollection();

            ServiceProvider = services
                .AddMarketDataClient(b => b.Configure(o => o.BaseAddress = new Uri("https://api-public.sandbox.pro.coinbase.com", UriKind.Absolute)))
                .BuildServiceProvider();

            MarketDataClient = ServiceProvider.GetRequiredService<IMarketDataClient>();

            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            await ServiceProvider.DisposeAsync();
        }
    }
}
