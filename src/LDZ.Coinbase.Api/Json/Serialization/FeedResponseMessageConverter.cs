using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using LDZ.Coinbase.Api.Model.Feed;

namespace LDZ.Coinbase.Api.Json.Serialization
{
    internal class FeedResponseMessageConverter : JsonConverter<FeedResponseMessage>
    {
        public override FeedResponseMessage? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            var propertyName = reader.GetString();
            if (propertyName != "type")
            {
                throw new JsonException();
            }

            reader.Read();
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }

            var messageType = reader.GetString();
            return messageType switch
            {
                "heartbeat" => ReadHeartbeatMessage(ref reader, new HeartbeatMessage()),
                _ => throw new JsonException($"Message of type \"{messageType}\" is NOT supported.")
            };
        }

        private static HeartbeatMessage ReadHeartbeatMessage(ref Utf8JsonReader reader, HeartbeatMessage value)
        {
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return value;
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    reader.Read();
                    switch (propertyName)
                    {
                        case "product_id":
                            value.ProductId = reader.GetString();
                            break;

                        case "time":
                            value.Time = reader.GetDateTime();
                            break;

                        case "last_trade_id":
                            value.LastTradeId = reader.GetInt64();
                            break;

                        case "sequence":
                            value.Sequence = reader.GetInt64();
                            break;
                    }
                }

            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, FeedResponseMessage value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
