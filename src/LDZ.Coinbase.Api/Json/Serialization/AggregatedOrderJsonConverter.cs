using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using LDZ.Coinbase.Api.Model.MarketData;

namespace LDZ.Coinbase.Api.Json.Serialization
{
    public class AggregatedOrderJsonConverter : JsonConverter<AggregatedProductOrder>
    {
        public override AggregatedProductOrder Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException();
            }

            // Price
            if (!reader.Read() || !decimal.TryParse(reader.GetString(), out var price))
            {
                throw new JsonException();
            }

            // Size
            if (!reader.Read() || !decimal.TryParse(reader.GetString(), out var size))
            {
                throw new JsonException();
            }

            // NumOrders
            if (!reader.Read() || !reader.TryGetInt32(out var numOrders))
            {
                throw new JsonException();
            }

            if (reader.Read() && reader.TokenType == JsonTokenType.EndArray)
            {
                return new AggregatedProductOrder {Price = price, Size = size, NumOrders = numOrders};
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, AggregatedProductOrder value, JsonSerializerOptions options)
            => throw new NotImplementedException();
    }
}
