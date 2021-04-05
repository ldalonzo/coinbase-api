﻿using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using LDZ.Coinbase.Api.Model.Feed;

namespace LDZ.Coinbase.Api.Json.Serialization
{
    internal class FeedMessageConverter : JsonConverter<FeedMessage>
    {
        public override FeedMessage? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, FeedMessage value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            if (value is SubscribeMessage subscribe)
            {
                writer.WriteString("type", "subscribe");

                if (subscribe.Channels != null)
                {
                    writer.WriteStartArray("channels");
                    foreach (var channel in subscribe.Channels)
                    {
                        JsonSerializer.Serialize(writer, channel, options);
                    }
                }

                writer.WriteEndArray();
            }

            writer.WriteEndObject();
        }
    }
}
