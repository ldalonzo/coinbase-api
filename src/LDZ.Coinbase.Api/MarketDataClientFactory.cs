using LDZ.Coinbase.Api.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace LDZ.Coinbase.Api
{
    public static class MarketDataClientFactory
    {
        public static IMarketDataClient CreateClient() => new ServiceCollection()
            .AddCoinbaseProRestApi()
            .BuildServiceProvider()
            .GetRequiredService<IMarketDataClient>();
    }
}
