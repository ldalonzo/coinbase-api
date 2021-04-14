using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using LDZ.Coinbase.Api.Model;

namespace LDZ.Coinbase.Api.Json.Serialization
{
    public class OrderSideConverter : JsonConverter<OrderSide>
    {
        public override OrderSide Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => reader.GetString() switch
            {
                "buy" => OrderSide.Buy,
                "sell" => OrderSide.Sell,
                _ => throw new JsonException()
            };

        public override void Write(Utf8JsonWriter writer, OrderSide value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case OrderSide.Buy:
                    writer.WriteStringValue("buy");
                    break;
                case OrderSide.Sell:
                    writer.WriteStringValue("sell");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }
    }
}
