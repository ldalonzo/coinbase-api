using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using LDZ.Coinbase.Api.Model;

namespace LDZ.Coinbase.Api.Json.Serialization
{
    public class OrderTypeConverter : JsonConverter<OrderType>
    {
        public override OrderType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => reader.GetString() switch
            {
                "limit" => OrderType.Limit,
                "market" => OrderType.Market,
                _ => throw new JsonException()
            };


        public override void Write(Utf8JsonWriter writer, OrderType value, JsonSerializerOptions options)
            => throw new NotImplementedException();
    }
}
