using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using LDZ.Coinbase.Api.DependencyInjection;
using LDZ.Coinbase.Api.Model.Feed;
using LDZ.Coinbase.Api.Model.Feed.Channels;
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
                Channels = new List<Channel> {new HeartbeatChannel
                {
                    Products = new List<string> {"ETH-EUR"}
                }}
            };

            var json = JsonSerializer.Serialize<FeedRequestMessage>(message, SerializerOptions);

            var expected = await File.ReadAllTextAsync("TestData/subscribe_heartbeat.json");
            json.ShouldBe(expected);
        }

        [Fact]
        public async Task SerializeSubscribeMessageTickerChannel()
        {
            var message = new SubscribeMessage
            {
                Channels = new List<Channel> {new TickerChannel
                {
                    Products = new List<string> {"ETH-EUR","BTC-USD"}
                }}
            };

            var json = JsonSerializer.Serialize<FeedRequestMessage>(message, SerializerOptions);

            var expected = await File.ReadAllTextAsync("TestData/subscribe_ticker.json");
            json.ShouldBe(expected);
        }

        [Fact]
        public async Task SerializeSubscribeMessageLevel2Channel()
        {
            var message = new SubscribeMessage
            {
                Channels = new List<Channel> {new Level2Channel
                {
                    Products = new List<string> {"XTZ-EUR"}
                }}
            };

            var json = JsonSerializer.Serialize<FeedRequestMessage>(message, SerializerOptions);

            var expected = await File.ReadAllTextAsync("TestData/subscribe_level2.json");
            json.ShouldBe(expected);
        }

        [Fact]
        public async Task DeserializeSubscriptionsMessageHeartbeatChannel()
        {
            var actual = JsonSerializer.Deserialize<FeedResponseMessage>(await File.ReadAllTextAsync("TestData/message_subscriptions_heartbeat.json"), SerializerOptions);

            var message = actual.ShouldBeOfType<SubscriptionsMessage>();
            message.Channels.ShouldNotBeNull();
            var channel = message.Channels.ShouldHaveSingleItem().ShouldBeOfType<HeartbeatChannel>();
            channel.Products.ShouldHaveSingleItem().ShouldBe("ETH-EUR");
        }

        [Fact]
        public async Task DeserializeSubscriptionsMessageTickerChannel()
        {
            var actual = JsonSerializer.Deserialize<FeedResponseMessage>(await File.ReadAllTextAsync("TestData/message_subscriptions_ticker.json"), SerializerOptions);

            var message = actual.ShouldBeOfType<SubscriptionsMessage>();
            message.Channels.ShouldNotBeNull();
            var channel = message.Channels.ShouldHaveSingleItem().ShouldBeOfType<TickerChannel>();
            channel.Products.ShouldHaveSingleItem().ShouldBe("BTC-USD");
        }

        [Fact]
        public async Task DeserializeHeartbeatMessage()
        {
            var actual = JsonSerializer.Deserialize<FeedResponseMessage>(await File.ReadAllTextAsync("TestData/message_heartbeat.json"), SerializerOptions);

            var message = actual.ShouldBeOfType<HeartbeatMessage>();
            message.Sequence.ShouldBe(90);
            message.ProductId.ShouldBe("BTC-USD");
            message.Time.ShouldBe(DateTimeOffset.Parse("2014-11-07T08:19:28.464459Z"));
            message.LastTradeId.ShouldBe(20);
        }

        [Fact]
        public async Task DeserializeTickerMessage()
        {
            var actual = JsonSerializer.Deserialize<FeedResponseMessage>(await File.ReadAllTextAsync("TestData/message_ticker.json"), SerializerOptions);

            var message = actual.ShouldBeOfType<TickerMessage>();
            message.Sequence.ShouldBe(6469324659);
            message.ProductId.ShouldBe("ETH-EUR");
            message.Time.ShouldBe(DateTimeOffset.Parse("2021-04-06T20:47:44.767292Z"));
            message.TradeId.ShouldBe(17108801);
            message.Price.ShouldBe(1785.08m);
        }
    }
}
