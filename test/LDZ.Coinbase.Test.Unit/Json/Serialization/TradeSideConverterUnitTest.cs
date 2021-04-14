using System.Text.Json;
using LDZ.Coinbase.Api.Json.Serialization;
using LDZ.Coinbase.Api.Model.MarketData;
using Shouldly;
using Xunit;

namespace LDZ.Coinbase.Test.Unit.Json.Serialization
{
    public class TradeSideConverterUnitTest
    {
        public TradeSideConverterUnitTest()
        {
            Options = new JsonSerializerOptions();
            Options.Converters.Add(new TradeSideConverter());
        }

        private JsonSerializerOptions Options { get; }

        [Theory]
        [InlineData("buy", TradeSide.Buy)]
        [InlineData("sell", TradeSide.Sell)]
        public void DeserializeSucceeds(string side, TradeSide expected)
            => JsonSerializer.Deserialize<TradeSide>($"\"{side}\"", Options).ShouldBe(expected);

        [Theory]
        [InlineData("blah")]
        [InlineData("\"blah\"")]
        public void DeserializeFails(string json)
            => Should.Throw<JsonException>(() => JsonSerializer.Deserialize<TradeSide>(json, Options));
    }
}
