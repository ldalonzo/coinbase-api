using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using LDZ.Coinbase.Api;
using Microsoft.Extensions.DependencyInjection;
using RichardSzalay.MockHttp;
using Shouldly;
using Xunit;

namespace LDZ.Coinbase.Test.Unit
{
    public class TradingClientUnitTest
    {
        [Fact]
        public async Task ListOrders()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .When(HttpMethod.Get, "https://api.pro.coinbase.com/orders")
                .Respond("application/json", await File.ReadAllTextAsync("TestData/orders.json"));

            var client = CreateTradingClient(mockHttp);
            var actual = await client.ListOrdersAsync();

            actual.Count().ShouldBe(2);
            actual.ShouldContain(o => o.Id == Guid.Parse("d0c5340b-6d6c-49d9-b567-48c4bfca13d2"));
        }

        private ITradingClient CreateTradingClient(MockHttpMessageHandler mockHttp)
            => new ServiceCollection().CreateClient<ITradingClient>(mockHttp);
    }
}
