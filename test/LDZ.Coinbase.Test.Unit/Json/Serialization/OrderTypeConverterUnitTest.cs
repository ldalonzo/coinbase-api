using System.Text.Json;
using LDZ.Coinbase.Api.Json.Serialization;
using LDZ.Coinbase.Api.Model;
using Shouldly;
using Xunit;

namespace LDZ.Coinbase.Test.Unit.Json.Serialization
{
    public class OrderTypeConverterUnitTest : CustomJsonConverterUnitTest<OrderType, OrderTypeConverter>
    {
        [Theory]
        [InlineData("limit", OrderType.Limit)]
        [InlineData("market", OrderType.Market)]
        public void DeserializeSucceeds(string side, OrderType expected)
            => Deserialize($"\"{side}\"").ShouldBe(expected);

        [Theory]
        [InlineData("")]
        [InlineData("blah")]
        [InlineData("\"blah\"")]
        public void DeserializeFails(string json)
            => Should.Throw<JsonException>(() => Deserialize(json));
    }
}
