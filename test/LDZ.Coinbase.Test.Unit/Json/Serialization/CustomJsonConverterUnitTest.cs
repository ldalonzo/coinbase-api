using System.Text.Json;
using System.Text.Json.Serialization;

namespace LDZ.Coinbase.Test.Unit.Json.Serialization
{
    public abstract class CustomJsonConverterUnitTest<T, TConverter>
        where TConverter : JsonConverter<T>, new()
    {
        protected string Serialize(T value)
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new TConverter());

            return JsonSerializer.Serialize(value, options);
        }

        protected T Deserialize(string json)
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new TConverter());

            return JsonSerializer.Deserialize<T>(json, options);
        }

        protected T Roundtrip(T value)
            => Deserialize(Serialize(value));
    }
}
