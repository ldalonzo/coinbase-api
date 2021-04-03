using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using LDZ.Coinbase.Api;
using LDZ.Coinbase.Api.Model;
using Microsoft.Extensions.DependencyInjection;
using RichardSzalay.MockHttp;
using Shouldly;
using Xunit;

namespace LDZ.Coinbase.Test.Unit
{
    public class TradingClientUnitTest
    {
        [Fact]
        public async Task PlaceNewOrder()
        {
            var id = Guid.Parse("d0c5340b-6d6c-49d9-b567-48c4bfca13d2");

            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .When(HttpMethod.Post, "https://api.pro.coinbase.com/orders")
                .WithContent(await File.ReadAllTextAsync($"TestData/new-order-request_{id}.json"))
                .Respond("application/json", await File.ReadAllTextAsync($"TestData/get_orders_{id}.json"));

            var client = CreateTradingClient(mockHttp);
            var actual = await client.PlaceNewOrderAsync(new NewOrderParameters
            {
                ProductId = "BTC-USD",
                Price = 0.100m,
                Side = OrderSide.Buy,
                Size = 0.01m
            });

            actual.ShouldNotBeNull();
            actual.Id.ShouldBe(id);
            actual.ProductId.ShouldBe("BTC-USD");
            actual.Size.ShouldBe(0.01m);
        }

        [Fact]
        public async Task ListOrders()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .When(HttpMethod.Get, "https://api.pro.coinbase.com/orders")
                .Respond("application/json", await File.ReadAllTextAsync("TestData/get_orders.json"));

            var client = CreateTradingClient(mockHttp);
            var actual = await client.ListOrdersAsync();

            actual.Count().ShouldBe(2);
            actual.ShouldContain(o => o.Id == Guid.Parse("d0c5340b-6d6c-49d9-b567-48c4bfca13d2"));
        }

        [Theory]
        [AutoData]
        public async Task CancelOrder(Guid orderId)
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .When(HttpMethod.Delete, $"https://api.pro.coinbase.com/orders/{orderId}")
                .Respond("application/json", $"\"{orderId}\"");

            var client = CreateTradingClient(mockHttp);
            var actual = await client.CancelOrder(orderId);
            actual.ShouldBe(orderId);
        }

        [Fact]
        public async Task CancelAllOrders()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .When(HttpMethod.Delete, $"https://api.pro.coinbase.com/orders")
                .Respond("application/json", await File.ReadAllTextAsync("TestData/delete_orders.json"));

            var client = CreateTradingClient(mockHttp);
            var actual = await client.CancelAllOrders();
            actual.ShouldContain(c => c == Guid.Parse("144c6f8e-713f-4682-8435-5280fbe8b2b4"));
        }

        [Fact]
        public async Task GetOrder_Settled()
        {
            var id = Guid.Parse("68e6a28f-ae28-4788-8d4f-5ab4e5e5ae08");
            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .When(HttpMethod.Get, $"https://api.pro.coinbase.com/orders/{id}")
                .Respond("application/json", await File.ReadAllTextAsync($"TestData/get_orders_{id}.json"));

            var client = CreateTradingClient(mockHttp);
            var actual = await client.GetOrderAsync(id);

            actual.Id.ShouldBe(id);
            actual.Price.ShouldBeNull();
            actual.Size.ShouldBe(1.00000000m);
            actual.ProductId.ShouldBe("BTC-USD");
            actual.Side.ShouldBe(OrderSide.Buy);
            actual.Funds.ShouldBe(9.9750623400000000m);
            actual.SpecifiedFunds.ShouldBe(10.0000000000000000m);
            actual.Type.ShouldBe(OrderType.Market);
            actual.PostOnly.ShouldBe(false);
            actual.CreatedAt.ShouldBe(DateTime.Parse("2016-12-08T20:09:05.508883Z"));
            actual.DoneAt.ShouldBe(DateTime.Parse("2016-12-08T20:09:05.527Z"));
            actual.FillFees.ShouldBe(0.0249376391550000m);
            actual.FilledSize.ShouldBe(0.01291771m);
            actual.ExecutedValue.ShouldBe(9.9750556620000000m);
            actual.Settled.ShouldBe(true);
        }

        [Fact]
        public async Task GetOrderUnsettled()
        {
            var id = Guid.Parse("d0c5340b-6d6c-49d9-b567-48c4bfca13d2");
            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .When(HttpMethod.Get, $"https://api.pro.coinbase.com/orders/{id}")
                .Respond("application/json", await File.ReadAllTextAsync($"TestData/get_orders_{id}.json"));

            var client = CreateTradingClient(mockHttp);
            var actual = await client.GetOrderAsync(id);

            actual.Id.ShouldBe(id);
            actual.Price.ShouldBe(0.1m);
            actual.Size.ShouldBe(0.010000000m);
            actual.ProductId.ShouldBe("BTC-USD");
            actual.Side.ShouldBe(OrderSide.Buy);
            actual.Funds.ShouldBeNull();
            actual.SpecifiedFunds.ShouldBeNull();
            actual.Type.ShouldBe(OrderType.Limit);
            actual.PostOnly.ShouldBe(false);
            actual.CreatedAt.ShouldBe(DateTime.Parse("2016-12-08T20:02:28.53864Z"));
            actual.DoneAt.ShouldBeNull();
            actual.FillFees.ShouldBe(0m);
            actual.FilledSize.ShouldBe(0m);
            actual.ExecutedValue.ShouldBe(0m);
            actual.Settled.ShouldBe(false);
        }

        private ITradingClient CreateTradingClient(MockHttpMessageHandler mockHttp)
            => new ServiceCollection().CreateClient<ITradingClient>(mockHttp);
    }
}
