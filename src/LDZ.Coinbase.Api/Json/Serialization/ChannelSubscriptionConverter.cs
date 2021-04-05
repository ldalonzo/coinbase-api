using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using LDZ.Coinbase.Api.Model.Feed.Channel;

namespace LDZ.Coinbase.Api.Json.Serialization
{
    internal class ChannelSubscriptionConverter : JsonConverter<ChannelSubscription>
    {
        public override ChannelSubscription? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, ChannelSubscription value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            if (value is HeartbeatChannelSubscription heartbeatCh)
            {
                writer.WriteString("name", "heartbeat");

                if (heartbeatCh.Products != null)
                {
                    writer.WriteStartArray("product_ids");

                    foreach (var id in heartbeatCh.Products)
                    {
                        writer.WriteStringValue(id);
                    }
                }

                writer.WriteEndArray();
            }

            writer.WriteEndObject();
        }
    }
}
