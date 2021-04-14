using System.Text.Json;
using LDZ.Coinbase.Api.Json.Serialization;
using LDZ.Coinbase.Api.Model;
using Shouldly;
using Xunit;

namespace LDZ.Coinbase.Test.Unit.Json.Serialization
{
    public class OrderTypeConverterUnitTest
    {
        public OrderTypeConverterUnitTest()
        {
            Options = new JsonSerializerOptions();
            Options.Converters.Add(new OrderTypeConverter());
        }

        private JsonSerializerOptions Options { get; }

        [Theory]
        [InlineData("limit", OrderType.Limit)]
        [InlineData("market", OrderType.Market)]
        public void DeserializeSucceeds(string side, OrderType expected)
            => JsonSerializer.Deserialize<OrderType>($"\"{side}\"", Options).ShouldBe(expected);

        [Theory]
        [InlineData("")]
        [InlineData("blah")]
        [InlineData("\"blah\"")]
        public void DeserializeFails(string json)
            => Should.Throw<JsonException>(() => JsonSerializer.Deserialize<OrderType>(json, Options));
    }
}
