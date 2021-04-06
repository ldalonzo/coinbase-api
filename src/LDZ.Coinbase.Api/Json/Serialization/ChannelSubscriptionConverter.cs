using System;
using System.Collections.Generic;
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

                    reader.Read();
                    switch (propertyName)
                    {
                        case "product_ids":

                            value.Products = new List<string>();

                            if (reader.TokenType != JsonTokenType.StartArray)
                            {
                                throw new JsonException();
                            }

                            reader.Read();
                            while (reader.TokenType != JsonTokenType.EndArray)
                            {
                                if (reader.TokenType != JsonTokenType.String)
                                {
                                    throw new JsonException();
                                }

                                value.Products.Add(reader.GetString());
                                reader.Read();
                            }

                            break;
                    }
                }

            }

            throw new JsonException();
        }
    }
}
