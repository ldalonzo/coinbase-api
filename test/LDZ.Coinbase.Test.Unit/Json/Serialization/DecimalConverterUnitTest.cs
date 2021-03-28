using System.Text.Json;
using AutoFixture.Xunit2;
using LDZ.Coinbase.Api.Json.Serialization;
using Shouldly;
using Xunit;

namespace LDZ.Coinbase.Test.Unit.Json.Serialization
{
    public class DecimalConverterUnitTest : CustomJsonConverterUnitTest<decimal, DecimalConverter>
    {
        [Theory]
        [AutoData]
        public void DeserializeSucceeds(decimal expected)
            => Deserialize($"\"{expected}\"").ShouldBe(expected);

        [Theory]
        [InlineData("")]
        [InlineData("\"")]
        [InlineData("\"\"")]
        [InlineData("\"blah\"")]
        [InlineData("\"blah")]
        public void DeserializeFails(string json)
            => Should.Throw<JsonException>(() => Deserialize(json));
    }
}
