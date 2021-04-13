using System.Text.Json;
using AutoFixture.Xunit2;
using LDZ.Coinbase.Api.Json.Serialization;
using Shouldly;
using Xunit;

namespace LDZ.Coinbase.Test.Unit.Json.Serialization
{
    public class DecimalConverterUnitTest
    {
        public DecimalConverterUnitTest()
        {
            Options = new JsonSerializerOptions();
            Options.Converters.Add(new DecimalConverter());
        }

        private JsonSerializerOptions Options { get; }

        [Theory]
        [AutoData]
        public void DeserializeSucceeds(decimal expected)
            => JsonSerializer.Deserialize<decimal>($"\"{expected}\"", Options).ShouldBe(expected);

        [Theory]
        [InlineData("")]
        [InlineData("\"")]
        [InlineData("\"\"")]
        [InlineData("\"blah\"")]
        [InlineData("\"blah")]
        public void DeserializeFails(string json)
            => Should.Throw<JsonException>(() => JsonSerializer.Deserialize<decimal>(json, Options));

        [Theory]
        [AutoData]
        public void RoundtripSucceeds(decimal expected)
            => JsonSerializer.Deserialize<decimal>(JsonSerializer.Serialize(expected, Options), Options).ShouldBe(expected);
    }
}
