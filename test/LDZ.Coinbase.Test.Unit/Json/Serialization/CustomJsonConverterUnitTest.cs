using System.Text.Json;
using System.Text.Json.Serialization;

namespace LDZ.Coinbase.Test.Unit.Json.Serialization
{
    public abstract class CustomJsonConverterUnitTest<T, TConverter>
        where TConverter : JsonConverter<T>, new()
    {
        protected CustomJsonConverterUnitTest()
        {
            Options = new JsonSerializerOptions();
            Options.Converters.Add(new TConverter());
        }

        private JsonSerializerOptions Options { get; }

        protected string Serialize(T value)
            => JsonSerializer.Serialize(value, Options);

        protected T Deserialize(string json)
            => JsonSerializer.Deserialize<T>(json, Options);

        protected T Roundtrip(T value)
            => Deserialize(Serialize(value));
    }
}
