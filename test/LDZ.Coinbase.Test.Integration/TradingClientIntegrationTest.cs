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
            _fixture = fixture;
            _helper = helper;

        }

        private readonly CoinbaseRestApiFixture _fixture;
        private readonly ITestOutputHelper _helper;

        private IServiceScope _serviceScope;

        private IMarketDataClient MarketData { get; set; }
        private ITradingClient TradingClient { get; set; }

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
                Size = product.BaseMinSize,
                Price = productOrderBook.GetWorstBid()
            });
            newOrder.ShouldNotBeNull();
            _helper.WriteLine($"Placed order {newOrder}");

            var cancelledOrderId = await TradingClient.CancelOrder(newOrder.Id, productId);
            cancelledOrderId.ShouldBe(newOrder.Id);
        }

        public Task InitializeAsync()
        {
            _serviceScope = _fixture.ServiceProvider.CreateScope();

            MarketData = _serviceScope.ServiceProvider.GetRequiredService<IMarketDataClient>();
            TradingClient = _serviceScope.ServiceProvider.GetRequiredService<ITradingClient>();
            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            _serviceScope.Dispose();
            return Task.CompletedTask;
        }
    }
}
