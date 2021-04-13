using System.Threading.Tasks;
using LDZ.Coinbase.Api;
using LDZ.Coinbase.Api.Model;
using LDZ.Coinbase.Api.Model.MarketData;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace LDZ.Coinbase.Test.Integration
{
    [Collection(nameof(CoinbaseRestApiCollection))]
    public class TradingClientIntegrationTest
    {
        public TradingClientIntegrationTest(CoinbaseRestApiFixture fixture, ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;

            MarketData = fixture.ApiFactory.CreateMarketDataClient();
            TradingClient = fixture.ApiFactory.CreateTradingClient();
        }

        private readonly ITestOutputHelper _testOutput;

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
                Price = productOrderBook?.GetWorstBid() - product?.QuoteIncrement
            });
            newOrder.ShouldNotBeNull();
            _testOutput.WriteLine($"Placed order {newOrder}");

            var cancelledOrderId = await TradingClient.CancelOrder(newOrder.Id, productId);
            cancelledOrderId.ShouldBe(newOrder.Id);
        }
    }
}
