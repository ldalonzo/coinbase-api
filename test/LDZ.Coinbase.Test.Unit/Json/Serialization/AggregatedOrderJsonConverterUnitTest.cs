using System.Text.Json;
using AutoFixture.Xunit2;
using LDZ.Coinbase.Api.Json.Serialization;
using LDZ.Coinbase.Api.Model.MarketData;
using Shouldly;
using Xunit;

namespace LDZ.Coinbase.Test.Unit.Json.Serialization
{
    public class AggregatedOrderJsonConverterUnitTest : CustomJsonConverterUnitTest<AggregatedProductOrder, AggregatedOrderJsonConverter>
    {
        [Theory]
        [AutoData]
        public void DeserializeSucceeds(decimal price, decimal size, int numOrders)
        {
            var actual = Deserialize($"[\"{price}\",\"{size}\",{numOrders}]");

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
            => Should.Throw<JsonException>(() => Deserialize(json));
    }
}
