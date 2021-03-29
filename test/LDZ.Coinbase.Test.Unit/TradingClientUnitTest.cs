using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using LDZ.Coinbase.Api;
using LDZ.Coinbase.Api.Model.MarketData;
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

        [Fact]
        public async Task GetOrder()
        {
            var id = Guid.Parse("68e6a28f-ae28-4788-8d4f-5ab4e5e5ae08");
            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .When(HttpMethod.Get, $"https://api.pro.coinbase.com/orders/{id}")
                .Respond("application/json", await File.ReadAllTextAsync($"TestData/orders_{id}.json"));

            var client = CreateTradingClient(mockHttp);
            var actual = await client.GetOrderAsync(id);

            actual.Id.ShouldBe(id);
            actual.Size.ShouldBe(1.00000000m);
            actual.ProductId.ShouldBe("BTC-USD");
            actual.Side.ShouldBe(TradeSide.Buy);
            actual.Funds.ShouldBe(9.9750623400000000m);
            actual.SpecifiedFunds.ShouldBe(10.0000000000000000m);
            actual.PostOnly.ShouldBe(false);
            actual.CreatedAt.ShouldBe(DateTime.Parse("2016-12-08T20:09:05.508883Z"));
            actual.DoneAt.ShouldBe(DateTime.Parse("2016-12-08T20:09:05.527Z"));
            actual.FillFees.ShouldBe(0.0249376391550000m);
            actual.FilledSize.ShouldBe(0.01291771m);
            actual.ExecutedValue.ShouldBe(9.9750556620000000m);
            actual.Settled.ShouldBe(true);
        }

        private ITradingClient CreateTradingClient(MockHttpMessageHandler mockHttp)
            => new ServiceCollection().CreateClient<ITradingClient>(mockHttp);
    }
}
