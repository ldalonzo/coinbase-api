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

            var services = new ServiceCollection();

            var client = services
                .AddMarketDataClient()
                .AddMockHttpClient(mockHttp)
                .BuildServiceProvider()
                .GetRequiredService<IMarketDataClient>();

            var actual = await client.GetProductsAsync();

            Assert.Contains(actual, p => p.Id == "BTC-EUR");
        }
    }
}
