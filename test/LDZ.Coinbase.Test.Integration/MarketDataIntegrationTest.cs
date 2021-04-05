using System;
using System.Linq;
using System.Threading.Tasks;
using LDZ.Coinbase.Api;
using LDZ.Coinbase.Api.Model.MarketData;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace LDZ.Coinbase.Test.Integration
{
    [Collection(nameof(CoinbaseRestApiCollection))]
    public class MarketDataIntegrationTest : IAsyncLifetime
    {
        public MarketDataIntegrationTest(CoinbaseRestApiFixture fixture)
        {
            _fixture = fixture;
        }

        private readonly CoinbaseRestApiFixture _fixture;
        private IServiceScope _serviceScope;

        private IMarketDataClient MarketData { get; set; }

        [Fact]
        public async Task GetProducts()
        {
            var actualProducts = await MarketData.GetProductsAsync();

            actualProducts.ShouldNotBeEmpty();
            actualProducts.ShouldContain(p => p.Id == "BTC-EUR");
        }

        [Theory]
        [InlineData("BTC-EUR")]
        [InlineData("BTC-USD")]
        public async Task GetProduct(string productId)
        {
            var actual = await MarketData.GetProductAsync(productId);

            Assert.NotNull(actual);
            Assert.Equal(productId, actual.Id);
        }

        [Theory]
        [InlineData("BTC-USD")]
        public async Task GetTrades(string productId)
        {
            var actual = await MarketData.GetTradesAsync(productId);

            Assert.NotEmpty(actual);
            Assert.True(actual.After.HasValue);
            Assert.True(actual.Before.HasValue);
        }

        [Theory]
        [InlineData("BTC-USD")]
        public async Task GetTradesWithAfterCursor(string productId)
        {
            var page1 = await MarketData.GetTradesAsync(productId);
            var page2 = await MarketData.GetTradesAsync(productId, page1.After);

            page1.ShouldNotBeEmpty();
            page2.ShouldNotBeEmpty();
            page1.Intersect(page2, new TradeTradeIdEqualityComparer()).ShouldBeEmpty();
        }

        [Theory]
        [InlineData("BTC-USD")]
        public async Task GetProductOrderBook(string productId)
        {
            var actual = await MarketData.GetProductOrderBookAsync(productId);

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
            var actual = await MarketData.GetProductOrderBookAsync(productId, AggregatedProductOrderBookLevel.LevelTwo);

            actual.Sequence.ShouldBePositive();

            actual.Bids.ShouldNotBeEmpty();
            actual.Bids.Count().ShouldBe(50);

            actual.Asks.ShouldNotBeEmpty();
            actual.Asks.Count().ShouldBe(50);
        }

        [Fact]
        public async Task GetTime()
        {
            var actual = await MarketData.GetTimeAsync();

            actual.Epoch.ShouldBePositive();
            actual.Iso.Date.ShouldBe(DateTime.Today.Date);
        }

        public Task InitializeAsync()
        {
            _serviceScope = _fixture.ServiceProvider.CreateScope();
            MarketData = _serviceScope.ServiceProvider.GetRequiredService<IMarketDataClient>();

            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            _serviceScope.Dispose();

            return Task.CompletedTask;
        }
    }
}
