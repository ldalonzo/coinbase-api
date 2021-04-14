using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using LDZ.Coinbase.Api.Model.Feed.Channels;

namespace LDZ.Coinbase.Api.Json.Serialization
{
    internal class FeedChannelConverter : JsonConverter<FeedChannel>
    {
        private const string Name = "name";

        public override FeedChannel? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
                ChannelSubscriptionNames.Heartbeat => Deserialize<HeartbeatChannel>(ref reader, options),
                ChannelSubscriptionNames.Level2 => Deserialize<Level2Channel>(ref reader, options),
                ChannelSubscriptionNames.Ticker => Deserialize<TickerChannel>(ref reader, options),
                _ => throw new JsonException()
            };
        }

        public override void Write(Utf8JsonWriter writer, FeedChannel value, JsonSerializerOptions options)
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

        private static TChannel? Deserialize<TChannel>(ref Utf8JsonReader reader, JsonSerializerOptions options)
            where TChannel : ProductsChannel, new()
            => new ProductsChannelConverter<TChannel>().Read(ref reader, typeof(TChannel), options);

        private class ProductsChannelConverter<TChannel> : JsonConverter<TChannel>
            where TChannel : ProductsChannel, new()
        {
            public override TChannel? Read(ref Utf8JsonReader reader, Type typeToConvert,
                JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.String)
                {
                    throw new JsonException();
                }

                var value = new TChannel();

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        return value;
                    }

                    if (reader.TokenType == JsonTokenType.PropertyName)
                    {
                        var propertyName = reader.GetString();
                        if (propertyName == ProductArrayConverter.ProductIds)
                        {
                            value.Products = ProductArrayConverter.Deserialize(ref reader, options);
                        }
                    }
                }

                throw new JsonException();
            }

            public override void Write(Utf8JsonWriter writer, TChannel value, JsonSerializerOptions options)
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

                    var product = reader.GetString();
                    if (string.IsNullOrWhiteSpace(product))
                    {
                        throw new JsonException();
                    }

                    products.Add(product);

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
