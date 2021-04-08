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
                FeedResponseMessageNames.Error => ErrorMessageConverter.Deserialize(ref reader, options),
                FeedResponseMessageNames.Heartbeat => HeartbeatMessageConverter.Deserialize(ref reader, options),
                FeedResponseMessageNames.Subscriptions => SubscriptionsMessageConverter.Deserialize(ref reader, options),
                FeedResponseMessageNames.Ticker => TickerMessageConverter.Deserialize(ref reader, options),
                _ => throw new JsonException($"Message of type \"{messageType}\" is NOT supported.")
            };
        }

        public override void Write(Utf8JsonWriter writer, FeedResponseMessage value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        private class ErrorMessageConverter : JsonConverter<ErrorMessage>
        {
            public static ErrorMessage? Deserialize(ref Utf8JsonReader reader, JsonSerializerOptions options)
                => new ErrorMessageConverter().Read(ref reader, typeof(ErrorMessageConverter), options);

            public override ErrorMessage? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.String)
                {
                    throw new JsonException();
                }

                if (reader.GetString() != FeedResponseMessageNames.Error)
                {
                    throw new JsonException();
                }

                var value = new ErrorMessage();

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
                            case "message":
                                value.Message = reader.GetString();
                                break;

                            case "reason":
                                value.Reason = reader.GetString();
                                break;
                        }
                    }

                }

                throw new JsonException();
            }

            public override void Write(Utf8JsonWriter writer, ErrorMessage value, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }
        }

        private class HeartbeatMessageConverter : JsonConverter<HeartbeatMessage>
        {
            public static HeartbeatMessage? Deserialize(ref Utf8JsonReader reader, JsonSerializerOptions options)
                => new HeartbeatMessageConverter().Read(ref reader, typeof(HeartbeatMessage), options);

            public override HeartbeatMessage? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.String)
                {
                    throw new JsonException();
                }

                if (reader.GetString() != FeedResponseMessageNames.Heartbeat)
                {
                    throw new JsonException();
                }

                var value = new HeartbeatMessage();

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

            public override void Write(Utf8JsonWriter writer, HeartbeatMessage value, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }
        }

        private class SubscriptionsMessageConverter : JsonConverter<SubscriptionsMessage>
        {
            public static SubscriptionsMessage? Deserialize(ref Utf8JsonReader reader, JsonSerializerOptions options)
                => new SubscriptionsMessageConverter().Read(ref reader, typeof(SubscriptionsMessage), options);

            public override SubscriptionsMessage? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var value = new SubscriptionsMessage();

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

            public override void Write(Utf8JsonWriter writer, SubscriptionsMessage value, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }
        }

        private class TickerMessageConverter : JsonConverter<TickerMessage>
        {
            public static TickerMessage? Deserialize(ref Utf8JsonReader reader, JsonSerializerOptions options)
                => new TickerMessageConverter().Read(ref reader, typeof(TickerMessage), options);

            public override TickerMessage? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var value = new TickerMessage();

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

            public override void Write(Utf8JsonWriter writer, TickerMessage value, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }
        }
    }
}
