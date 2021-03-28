using System.Text.Json;
using System.Text.Json.Serialization;

namespace LDZ.Coinbase.Test.Unit.Json.Serialization
{
    public abstract class CustomJsonConverterUnitTest<T, TConverter>
        where TConverter : JsonConverter<T>, new()
    {
        protected T Deserialize(string json)
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new TConverter());

            return JsonSerializer.Deserialize<T>(json, options);
        }
    }
}