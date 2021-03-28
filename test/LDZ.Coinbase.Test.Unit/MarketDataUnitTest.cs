using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using LDZ.Coinbase.Api;
using LDZ.Coinbase.Api.DependencyInjection;
using LDZ.Coinbase.Api.Model.MarketData;
using Microsoft.Extensions.DependencyInjection;
using RichardSzalay.MockHttp;
using Shouldly;
using Xunit;

namespace LDZ.Coinbase.Test.Unit
{
    public class MarketDataUnitTest
    {
        [Fact]
        public async Task GetProducts()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .When("https://api.pro.coinbase.com/products")
                .Respond("application/json", await File.ReadAllTextAsync("TestData/products.json"));

            var client = CreateClient(mockHttp);
            var actual = await client.GetProductsAsync();

            Assert.Contains(actual, p => p.Id == "BTC-EUR");
        }

        [Theory]
        [InlineData("BTC-USD")]
        public async Task GetProduct(string productId)
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .When($"https://api.pro.coinbase.com/products/{productId}")
                .Respond("application/json", await File.ReadAllTextAsync($"TestData/products_{productId}.json"));

            var client = CreateClient(mockHttp);
            var actual = await client.GetProductAsync(productId);

            Assert.NotNull(actual);
            Assert.Equal("BTC-USD", actual.Id);
            Assert.Equal("BTC/USD", actual.DisplayName);
            Assert.Equal("BTC", actual.BaseCurrency);
            Assert.Equal("USD", actual.QuoteCurrency);
            Assert.Equal(1E-8m, actual.BaseIncrement);
            Assert.Equal(1E-2m, actual.QuoteIncrement);
            Assert.Equal(1E-3m, actual.BaseMinSize);
            Assert.Equal(280m, actual.BaseMaxSize);
        }

        [Theory]
        [InlineAutoData("BTC-USD")]
        public async Task GetTrades(string productId, int before, int after)
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .When($"https://api.pro.coinbase.com/products/{productId}/trades")
                .Respond(
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("CB-AFTER", $"{after}"),
                        new KeyValuePair<string, string>("CB-BEFORE", $"{before}")
                    },
                    "application/json",
                    await File.ReadAllTextAsync($"TestData/products_{productId}_trades.json"));

            var client = CreateClient(mockHttp);
            var actual = await client.GetTradesAsync(productId);

            Assert.NotEmpty(actual);
            Assert.Equal(after, actual.After);
            Assert.Equal(before, actual.Before);

            var actualTrade = actual.FirstOrDefault(a => a.TradeId == 73);
            Assert.Equal(TradeSide.Sell, actualTrade.Side);
            Assert.Equal(100m, actualTrade.Price);
            Assert.Equal(0.01m, actualTrade.Size);
            Assert.Equal(DateTime.Parse("2014-11-07T01:08:43.642366Z"), actualTrade.Time);
        }

        [Theory]
        [InlineData("BTC-USD")]
        public async Task GetProductOrderBook(string productId)
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .When($"https://api.pro.coinbase.com/products/{productId}/book")
                .Respond(
                    "application/json",
                    await File.ReadAllTextAsync($"TestData/products_{productId}_book.json"));

            var client = CreateClient(mockHttp);
            var actual = await client.GetProductOrderBook(productId);

            actual.Sequence.ShouldBe(3);

            var bestBid = actual.Bids.ShouldHaveSingleItem();
            bestBid.NumOrders.ShouldBe(2);
            bestBid.Price.ShouldBe(295.96m);
            bestBid.Size.ShouldBe(4.39088265m);

            var bestAsk = actual.Asks.ShouldHaveSingleItem();
            bestAsk.NumOrders.ShouldBe(12);
            bestAsk.Price.ShouldBe(295.97m);
            bestAsk.Size.ShouldBe(25.23542881m);
        }

        [Theory]
        [InlineData("BTC-USD")]
        public async Task GetProductOrderBookLevel2(string productId)
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .When($"https://api.pro.coinbase.com/products/{productId}/book?level=2")
                .Respond(
                    "application/json",
                    await File.ReadAllTextAsync($"TestData/products_{productId}_book_level2.json"));

            var client = CreateClient(mockHttp);
            var actual = await client.GetProductOrderBook(productId, AggregatedProductOrderBookLevel.LevelTwo);

            actual.Sequence.ShouldBePositive();

            actual.Bids.ShouldNotBeEmpty();
            actual.Bids.Count().ShouldBe(50);

            actual.Asks.ShouldNotBeEmpty();
            actual.Asks.Count().ShouldBe(50);
        }

        private static IMarketDataClient CreateClient(MockHttpMessageHandler mockHttp)
            => CreateClient(mockHttp, new ServiceCollection());

        private static IMarketDataClient CreateClient(MockHttpMessageHandler mockHttp, IServiceCollection services) => services
            .AddMarketDataClient()
            .AddMockHttpClient(mockHttp)
            .BuildServiceProvider()
            .GetRequiredService<IMarketDataClient>();
    }
}
