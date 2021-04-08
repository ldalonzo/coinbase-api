using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using LDZ.Coinbase.Api.Model.Feed.Channels;

namespace LDZ.Coinbase.Api.Json.Serialization
{
    internal class ChannelConverter : JsonConverter<Channel>
    {
        private const string Name = "name";

        public override Channel? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
                ChannelSubscriptionNames.Level2 => Level2ChannelConverter.Deserialize(ref reader, options),
                _ => throw new JsonException()
            };
        }

        public override void Write(Utf8JsonWriter writer, Channel value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            var productsWriter = new ProductArrayConverter();

            switch (value)
            {
                case HeartbeatChannel heartbeat:
                    writer.WriteString(Name, ChannelSubscriptionNames.Heartbeat);
                    productsWriter.Write(writer, heartbeat.Products, options);
                    break;

                case TickerChannel ticker:
                    writer.WriteString(Name, ChannelSubscriptionNames.Ticker);
                    productsWriter.Write(writer, ticker.Products, options);
                    break;

                case Level2Channel level2:
                    writer.WriteString(Name, ChannelSubscriptionNames.Level2);
                    productsWriter.Write(writer, level2.Products, options);
                    break;
            }

            writer.WriteEndObject();
        }

        private static Channel Read(ref Utf8JsonReader reader,  HeartbeatChannel value, JsonSerializerOptions options)
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
                        ProductArrayConverter.ProductIds => ProductArrayConverter.Deserialize(ref reader, options),
                        _ => value.Products
                    };
                }
            }

            throw new JsonException();
        }

        private static Channel Read(ref Utf8JsonReader reader, TickerChannel value, JsonSerializerOptions options)
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
                        ProductArrayConverter.ProductIds => ProductArrayConverter.Deserialize(ref reader, options),
                        _ => value.Products
                    };
                }
            }

            throw new JsonException();
        }

        private class Level2ChannelConverter : JsonConverter<Level2Channel>
        {
            public static Level2Channel? Deserialize(ref Utf8JsonReader reader, JsonSerializerOptions options)
                => new Level2ChannelConverter().Read(ref reader, typeof(Level2Channel), options);

            public override Level2Channel? Read(ref Utf8JsonReader reader, Type typeToConvert,
                JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.String)
                {
                    throw new JsonException();
                }

                if (reader.GetString() != FeedResponseMessageNames.Level2)
                {
                    throw new JsonException();
                }

                var value = new Level2Channel();

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
                            ProductArrayConverter.ProductIds => ProductArrayConverter.Deserialize(ref reader, options),
                            _ => value.Products
                        };
                    }
                }

                throw new JsonException();
            }

            public override void Write(Utf8JsonWriter writer, Level2Channel value, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }
        }

        private class ProductArrayConverter : JsonConverter<ICollection<string>>
        {
            public static ICollection<string>? Deserialize(ref Utf8JsonReader reader, JsonSerializerOptions options)
                => new ProductArrayConverter().Read(ref reader, typeof(IList<string>), options);

            public const string ProductIds = "product_ids";

            public override ICollection<string>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException();
                }

                if (reader.GetString() != ProductIds)
                {
                    throw new JsonException();
                }

                var products = new List<string>();

                reader.Read();
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

                    products.Add(reader.GetString());
                    reader.Read();
                }

                return products;
            }

            public override void Write(Utf8JsonWriter writer, ICollection<string>? value, JsonSerializerOptions options)
            {
                if (value != null)
                {
                    writer.WriteStartArray(ProductIds);

                    foreach (var id in value)
                    {
                        writer.WriteStringValue(id);
                    }

                    writer.WriteEndArray();
                }
            }
        }
    }
}
