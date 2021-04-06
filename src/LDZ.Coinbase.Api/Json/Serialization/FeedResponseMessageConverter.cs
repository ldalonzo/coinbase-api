using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using LDZ.Coinbase.Api.Model.Feed;
using LDZ.Coinbase.Api.Model.Feed.Channels;

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
                FeedResponseMessageNames.Heartbeat => ReadHeartbeatMessage(ref reader, options, new HeartbeatMessage()),
                FeedResponseMessageNames.Subscriptions => ReadSubscriptionsMessage(ref reader, options, new SubscriptionsMessage()),
                FeedResponseMessageNames.Ticker => ReadTickerMessage(ref reader, options, new TickerMessage()),
                _ => throw new JsonException($"Message of type \"{messageType}\" is NOT supported.")
            };
        }

        private static FeedResponseMessage ReadHeartbeatMessage(ref Utf8JsonReader reader, JsonSerializerOptions options, HeartbeatMessage value)
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

        private static FeedResponseMessage ReadSubscriptionsMessage(ref Utf8JsonReader reader, JsonSerializerOptions options, SubscriptionsMessage value)
        {
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return value;
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    switch (reader.GetString())
                    {
                        case "channels":

                            value.Channels = new List<Channel>();

                            reader.Read();
                            if (reader.TokenType != JsonTokenType.StartArray)
                            {
                                throw new JsonException();
                            }

                            reader.Read();
                            while (reader.TokenType != JsonTokenType.EndArray)
                            {
                                value.Channels.Add(JsonSerializer.Deserialize<Channel>(ref reader, options));
                                reader.Read();
                            }

                            break;
                    }
                }
            }

            throw new JsonException();
        }

        private static FeedResponseMessage ReadTickerMessage(ref Utf8JsonReader reader, JsonSerializerOptions options, TickerMessage value)
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

                        case "trade_id":
                            value.TradeId = reader.GetInt64();
                            break;

                        case "price":
                            value.Price = JsonSerializer.Deserialize<decimal>(ref reader, options);
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
