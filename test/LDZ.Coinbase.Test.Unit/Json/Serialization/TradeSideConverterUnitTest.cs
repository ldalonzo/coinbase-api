using System.Text.Json;
using LDZ.Coinbase.Api.Json.Serialization;
using LDZ.Coinbase.Api.Model.MarketData;
using Shouldly;
using Xunit;

namespace LDZ.Coinbase.Test.Unit.Json.Serialization
{
    public class TradeSideConverterUnitTest : CustomJsonConverterUnitTest<TradeSide, TradeSideConverter>
    {
        [Theory]
        [InlineData("buy", TradeSide.Buy)]
        [InlineData("sell", TradeSide.Sell)]
        public void DeserializeSucceeds(string side, TradeSide expected)
            => Deserialize($"\"{side}\"").ShouldBe(expected);

        [Theory]
        [InlineData("blah")]
        [InlineData("\"blah\"")]
        public void DeserializeFails(string json)
            => Should.Throw<JsonException>(() => Deserialize(json));
    }
}
