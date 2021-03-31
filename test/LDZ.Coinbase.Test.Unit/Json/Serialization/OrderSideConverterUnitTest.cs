using System.Text.Json;
using LDZ.Coinbase.Api.Json.Serialization;
using LDZ.Coinbase.Api.Model;
using Shouldly;
using Xunit;

namespace LDZ.Coinbase.Test.Unit.Json.Serialization
{
    public class OrderSideConverterUnitTest : CustomJsonConverterUnitTest<OrderSide, OrderSideConverter>
    {
        [Theory]
        [InlineData("buy", OrderSide.Buy)]
        [InlineData("sell", OrderSide.Sell)]
        public void DeserializeSucceeds(string side, OrderSide expected)
            => Deserialize($"\"{side}\"").ShouldBe(expected);

        [Theory]
        [InlineData("blah")]
        [InlineData("\"blah\"")]
        public void DeserializeFails(string json)
            => Should.Throw<JsonException>(() => Deserialize(json));

        [Theory]
        [InlineData(OrderSide.Buy)]
        [InlineData(OrderSide.Sell)]
        public void RoundtripSucceeds(OrderSide expected)
            => Roundtrip(expected).ShouldBe(expected);
    }
}
