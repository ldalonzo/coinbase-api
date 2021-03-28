using System.Linq;
using System.Threading.Tasks;
using LDZ.Coinbase.Api.Model.MarketData;
using Shouldly;
using Xunit;

namespace LDZ.Coinbase.Test.Integration
{
    [Collection(nameof(MarketDataCollection))]
    public class MarketDataIntegrationTest
    {
        private readonly MarketDataClientFixture _fixture;

        public MarketDataIntegrationTest(MarketDataClientFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetProducts()
        {
            var actualProducts = await _fixture.MarketDataClient.GetProductsAsync();

            actualProducts.ShouldNotBeEmpty();
            actualProducts.ShouldContain(p => p.Id == "BTC-EUR");
        }

        [Theory]
        [InlineData("BTC-EUR")]
        [InlineData("BTC-USD")]
        public async Task GetProduct(string productId)
        {
            var actual = await _fixture.MarketDataClient.GetProductAsync(productId);

            Assert.NotNull(actual);
            Assert.Equal(productId, actual.Id);
        }

        [Theory]
        [InlineData("BTC-USD")]
        public async Task GetTrades(string productId)
        {
            var actual = await _fixture.MarketDataClient.GetTradesAsync(productId);

            Assert.NotEmpty(actual);
            Assert.True(actual.After.HasValue);
            Assert.True(actual.Before.HasValue);
        }

        [Theory]
        [InlineData("BTC-USD")]
        public async Task GetTradesWithAfterCursor(string productId)
        {
            var page1 = await _fixture.MarketDataClient.GetTradesAsync(productId);
            var page2 = await _fixture.MarketDataClient.GetTradesAsync(productId, page1.After);

            page1.ShouldNotBeEmpty();
            page2.ShouldNotBeEmpty();
            page1.Intersect(page2, new TradeTradeIdEqualityComparer()).ShouldBeEmpty();
        }

        [Theory]
        [InlineData("BTC-USD")]
        public async Task GetProductOrderBook(string productId)
        {
            var actual = await _fixture.MarketDataClient.GetProductOrderBook(productId);

            actual.Sequence.ShouldBePositive();

            var bestBid = actual.Bids.ShouldHaveSingleItem();
            bestBid.NumOrders.ShouldBePositive();
            bestBid.Price.ShouldBePositive();
            bestBid.Size.ShouldBePositive();

            var bestAsk = actual.Asks.ShouldHaveSingleItem();
            bestAsk.NumOrders.ShouldBePositive();
            bestAsk.Price.ShouldBePositive();
            bestAsk.Size.ShouldBePositive();
        }

        [Theory]
        [InlineData("BTC-USD")]
        public async Task GetProductOrderBookLevel2(string productId)
        {
            var actual = await _fixture.MarketDataClient.GetProductOrderBook(productId, AggregatedProductOrderBookLevel.LevelTwo);

            actual.Sequence.ShouldBePositive();

            actual.Bids.ShouldNotBeEmpty();
            actual.Bids.Count().ShouldBe(50);

            actual.Asks.ShouldNotBeEmpty();
            actual.Asks.Count().ShouldBe(50);
        }
    }
}
