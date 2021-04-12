using System.Text.Json;
using LDZ.Coinbase.Api.Json.Serialization;
using LDZ.Coinbase.Api.Model;
using Shouldly;
using Xunit;

namespace LDZ.Coinbase.Test.Unit.Json.Serialization
{
    public class OrderSideConverterUnitTest
    {
        public OrderSideConverterUnitTest()
        {
            Options = new JsonSerializerOptions();
            Options.Converters.Add(new OrderSideConverter());
        }

        private JsonSerializerOptions Options { get; }

        [Theory]
        [InlineData("buy", OrderSide.Buy)]
        [InlineData("sell", OrderSide.Sell)]
        public void DeserializeSucceeds(string side, OrderSide expected)
            => JsonSerializer.Deserialize<OrderSide>($"\"{side}\"", Options).ShouldBe(expected);

        [Theory]
        [InlineData("blah")]
        [InlineData("\"blah\"")]
        public void DeserializeFails(string json)
            => Should.Throw<JsonException>(() => JsonSerializer.Deserialize<OrderSide>(json, Options));

        [Theory]
        [InlineData(OrderSide.Buy)]
        [InlineData(OrderSide.Sell)]
        public void RoundtripSucceeds(OrderSide expected)
            => JsonSerializer.Deserialize<OrderSide>(JsonSerializer.Serialize(expected, Options), Options).ShouldBe(expected);
    }
}
