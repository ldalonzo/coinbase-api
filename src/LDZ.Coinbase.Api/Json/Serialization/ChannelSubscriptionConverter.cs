using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using LDZ.Coinbase.Api.Model.Feed.Channel;

namespace LDZ.Coinbase.Api.Json.Serialization
{
    internal class ChannelSubscriptionConverter : JsonConverter<ChannelSubscription>
    {
        private const string Name = "name";

        public override ChannelSubscription? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
            if (propertyName != Name)
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
                ChannelSubscriptionNames.Heartbeat => Read(ref reader, new HeartbeatChannel(), options),
                ChannelSubscriptionNames.Ticker => Read(ref reader, new TickerChannel(), options),
                _ => throw new JsonException()
            };
        }

        public override void Write(Utf8JsonWriter writer, ChannelSubscription value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            if (value is HeartbeatChannel heartbeat)
            {
                writer.WriteString(Name, ChannelSubscriptionNames.Heartbeat);
                writer.WriteProducts(heartbeat.Products);
            }

            if (value is TickerChannel ticker)
            {
                writer.WriteString(Name, ChannelSubscriptionNames.Ticker);
                writer.WriteProducts(ticker.Products);
            }

            writer.WriteEndObject();
        }

        private static ChannelSubscription Read(ref Utf8JsonReader reader,  HeartbeatChannel value, JsonSerializerOptions options)
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
                    value.Products = propertyName switch
                    {
                        Utf8JsonWriterExtensions.ProductIds => Utf8JsonWriterExtensions.ReadProducts(ref reader, options),
                        _ => value.Products
                    };
                }
            }

            throw new JsonException();
        }

        private static ChannelSubscription Read(ref Utf8JsonReader reader, TickerChannel value, JsonSerializerOptions options)
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
                    value.Products = propertyName switch
                    {
                        Utf8JsonWriterExtensions.ProductIds => Utf8JsonWriterExtensions.ReadProducts(ref reader, options),
                        _ => value.Products
                    };
                }
            }

            throw new JsonException();
        }
    }
}
