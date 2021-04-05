using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using LDZ.Coinbase.Api.Model.MarketData;

namespace LDZ.Coinbase.Api.Json.Serialization
{
    public class TradeSideConverter : JsonConverter<TradeSide>
    {
        public override TradeSide Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => reader.GetString() switch
            {
                "buy" => TradeSide.Buy,
                "sell" => TradeSide.Sell,
                _ => throw new JsonException()
            };

        public override void Write(Utf8JsonWriter writer, TradeSide value, JsonSerializerOptions options)
            => throw new NotImplementedException();
    }
}
