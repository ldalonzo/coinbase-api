using System.Text.Json;
using AutoFixture.Xunit2;
using LDZ.Coinbase.Api.Json.Serialization;
using LDZ.Coinbase.Api.Model.MarketData;
using Shouldly;
using Xunit;

namespace LDZ.Coinbase.Test.Unit.Json.Serialization
{
    public class AggregatedOrderJsonConverterUnitTest
    {
        public AggregatedOrderJsonConverterUnitTest()
        {
            Options = new JsonSerializerOptions();
            Options.Converters.Add(new AggregatedOrderJsonConverter());
        }

        private JsonSerializerOptions Options { get; }

        [Theory]
        [AutoData]
        public void DeserializeSucceeds(decimal price, decimal size, int numOrders)
        {
            var json = $"[\"{price}\",\"{size}\",{numOrders}]";
            var actual = JsonSerializer.Deserialize<AggregatedProductOrder>(json, Options);

            actual.ShouldNotBeNull();
            actual.Price.ShouldBe(price);
            actual.Size.ShouldBe(size);
            actual.NumOrders.ShouldBe(numOrders);
        }

        [Theory]
        [InlineData("\"56160.93\",\"3009.97862195\",1]")]
        [InlineData("[\"blah\",\"3009.97862195\",1]")]
        [InlineData("[\"56160.93\",\"blah\",1]")]
        [InlineData("[\"56160.93\",\"3009.97862195\",1.2]")]
        [InlineData("[\"56160.93\",\"3009.97862195\",1")]
        [InlineData("[\"56160.93\",\"3009.97862195\",1,\"\"]")]
        public void DeserializeFails(string json)
            => Should.Throw<JsonException>(() => JsonSerializer.Deserialize<AggregatedProductOrder>(json, Options));
    }
}
