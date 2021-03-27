using System.IO;
using System.Threading.Tasks;
using LD.Coinbase.Api;
using LD.Coinbase.Api.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using RichardSzalay.MockHttp;
using Xunit;

namespace Test.LD.Coinbase.Api
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

        private static IMarketDataClient CreateClient(MockHttpMessageHandler mockHttp)
            => CreateClient(mockHttp, new ServiceCollection());

        private static IMarketDataClient CreateClient(MockHttpMessageHandler mockHttp, IServiceCollection services) => services
            .AddMarketDataClient()
            .AddMockHttpClient(mockHttp)
            .BuildServiceProvider()
            .GetRequiredService<IMarketDataClient>();
    }
}
