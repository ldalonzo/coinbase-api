using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LDZ.Coinbase.Api.Json.Serialization
{
    public class DecimalConverter : JsonConverter<decimal>
    {
        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (decimal.TryParse(reader.GetString(), out var value))
            {
                return value;
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
            => writer.WriteStringValue($"{value}");
    }
}
