using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using LDZ.Coinbase.Api.DependencyInjection;
using LDZ.Coinbase.Api.Model.Feed;
using LDZ.Coinbase.Api.Model.Feed.Channel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;

namespace LDZ.Coinbase.Feed.Test.Unit
{
    public class WebsocketFeedSerializationUnitTest
    {
        public WebsocketFeedSerializationUnitTest()
        {
            SerializerOptions = new ServiceCollection()
                .ConfigureCoinbaseSerializerOptions()
                .BuildServiceProvider()
                .GetRequiredService<IOptions<JsonSerializerOptions>>().Value;
        }

        private JsonSerializerOptions SerializerOptions { get; }

        [Fact]
        public async Task SerializeSubscribeMessageHeartbeatChannel()
        {
            var message = new SubscribeMessage
            {
                Channels = new List<ChannelSubscription> {new HeartbeatChannelSubscription
                {
                    Products = new List<string> {"ETH-EUR"}
                }}
            };

            var json = JsonSerializer.Serialize<FeedRequestMessage>(message, SerializerOptions);

            var expected = await File.ReadAllTextAsync("TestData/subscribe_heartbeat.json");
            json.ShouldBe(expected);
        }

        [Fact]
        public async Task DeserializeHeartbeatMessage()
        {
            var actual = JsonSerializer.Deserialize<FeedResponseMessage>(await File.ReadAllTextAsync("TestData/message_heartbeat.json"), SerializerOptions);

            var message = actual.ShouldBeOfType<HeartbeatMessage>();
            message.ProductId.ShouldBe("BTC-USD");
            message.Time.ShouldBe(DateTime.Parse("2014-11-07T08:19:28.464459Z"));
            message.LastTradeId.ShouldBe(20);
            message.Sequence.ShouldBe(90);
        }
    }
}
