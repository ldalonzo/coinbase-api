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
            var actual = await _fixture.MarketDataClient.GetProductsAsync();

            Assert.NotEmpty(actual);
            Assert.Contains(actual, p => p.Id == "BTC-EUR");
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

            Assert.NotEmpty(page1);
            Assert.NotEmpty(page2);
            page1.Intersect(page2, new TradeTradeIdEqualityComparer()).ShouldBeEmpty();
        }
    }
}
