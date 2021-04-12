using System.Threading.Tasks;
using LDZ.Coinbase.Api;
using LDZ.Coinbase.Api.Model;
using LDZ.Coinbase.Api.Model.MarketData;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace LDZ.Coinbase.Test.Integration
{
    [Collection(nameof(CoinbaseRestApiCollection))]
    public class TradingClientIntegrationTest : IAsyncLifetime
    {
        public TradingClientIntegrationTest(CoinbaseRestApiFixture fixture, ITestOutputHelper helper)
        {
            _helper = helper;

            ServiceScope = fixture.ServiceProvider.CreateScope();

            MarketData = ServiceScope.ServiceProvider.GetRequiredService<IMarketDataClient>();
            TradingClient = ServiceScope.ServiceProvider.GetRequiredService<ITradingClient>();
        }

        private readonly ITestOutputHelper _helper;

        private IServiceScope ServiceScope { get; }

        private IMarketDataClient MarketData { get; }
        private ITradingClient TradingClient { get; }

        [Theory]
        [InlineData("BTC-USD")]
        public async Task PlaceBuyLimitOrder(string productId)
        {
            var product = await MarketData.GetProductAsync(productId);
            var productOrderBook = await MarketData.GetProductOrderBookAsync(productId, AggregatedProductOrderBookLevel.LevelTwo);

            var newOrder = await TradingClient.PlaceNewOrderAsync(new NewOrderParameters
            {
                ProductId = productId,
                Side = OrderSide.Buy,
                Size = product?.BaseMinSize,
                Price = productOrderBook?.GetWorstBid()
            });
            newOrder.ShouldNotBeNull();
            _helper.WriteLine($"Placed order {newOrder}");

            var cancelledOrderId = await TradingClient.CancelOrder(newOrder.Id, productId);
            cancelledOrderId.ShouldBe(newOrder.Id);
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            ServiceScope.Dispose();
            return Task.CompletedTask;
        }
    }
}
