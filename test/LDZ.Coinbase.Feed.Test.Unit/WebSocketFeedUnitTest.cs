using System;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using LDZ.Coinbase.Api;
using LDZ.Coinbase.Api.DependencyInjection;
using LDZ.Coinbase.Api.Model.Feed;
using LDZ.Coinbase.Api.Net;
using LDZ.Coinbase.Api.Net.WebSockets;
using LDZ.Coinbase.Test.Shared;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace LDZ.Coinbase.Feed.Test.Unit
{
    public class WebSocketFeedUnitTest : IAsyncLifetime
    {
        public WebSocketFeedUnitTest(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        private readonly ITestOutputHelper _testOutput;

        private IWebSocketFeed? WebSocketFeed { get; set; }

        [Fact]
        public async Task SubscribeToHeartbeatChannel()
        {
            var webSocketMock = new Mock<IClientWebSocketFacade>();
            webSocketMock.SetupReceiveAsyncSequence()
                .Returns("TestData/message_BTC-USD_heartbeat.json");

            WebSocketFeed = CreateWebSocketFeed(webSocketMock);

            var spyTask = WebSocketFeed.SubscribeToHeartbeatChannel("BTC-USD").Spy(_testOutput, TimeSpan.FromMilliseconds(100));
            await WebSocketFeed.StartAsync();

            var spy = await spyTask;
            webSocketMock.Verify(socket => socket.ConnectAsync(It.Is<Uri>(u => u == EndpointUriNames.WebsocketFeedUri), It.IsAny<CancellationToken>()), Times.Exactly(1));
            webSocketMock.Verify(socket => socket.SendAsync(It.Is<ReadOnlyMemory<byte>>(buffer => buffer.ContainsUtf8String("\"heartbeat\",\"product_ids\":[\"BTC-USD\"]") ), It.Is<WebSocketMessageType>(t => t == WebSocketMessageType.Text), It.Is<bool>(b => b), It.IsAny<CancellationToken>()), Times.Exactly(1));

            spy.ReceivedMessages.OfType<HeartbeatMessage>().ShouldNotBeEmpty();
        }

        [Fact]
        public async Task SubscribeToTickerChannel()
        {
            var webSocketMock = new Mock<IClientWebSocketFacade>();
            webSocketMock.SetupReceiveAsyncSequence()
                .Returns("TestData/message_ETH-EUR_ticker.json");

            WebSocketFeed = CreateWebSocketFeed(webSocketMock);
            var spyTask = WebSocketFeed.SubscribeToTickerChannel("ETH-EUR").Spy(_testOutput, TimeSpan.FromMilliseconds(100));
            await WebSocketFeed.StartAsync();

            var spy = await spyTask;
            webSocketMock.Verify(socket => socket.ConnectAsync(It.Is<Uri>(u => u == EndpointUriNames.WebsocketFeedUri), It.IsAny<CancellationToken>()), Times.Exactly(1));
            webSocketMock.Verify(socket => socket.SendAsync(It.Is<ReadOnlyMemory<byte>>(buffer => buffer.ContainsUtf8String("\"ticker\",\"product_ids\":[\"ETH-EUR\"]")), It.Is<WebSocketMessageType>(t => t == WebSocketMessageType.Text), It.Is<bool>(j => j), It.IsAny<CancellationToken>()), Times.Exactly(1));
            spy.ReceivedMessages.OfType<TickerMessage>().ShouldHaveSingleItem();
        }

        [Fact]
        public async Task SubscribeToLevel2Channel()
        {
            var webSocketMock = new Mock<IClientWebSocketFacade>();
            webSocketMock
                .SetupReceiveAsyncSequence()
                .Returns("TestData/message_XTZ-EUR_snapshot.json")
                .Returns("TestData/message_XTZ-EUR_l2update_buy.json")
                .Returns("TestData/message_XTZ-EUR_l2update_sell.json");

            WebSocketFeed = CreateWebSocketFeed(webSocketMock);

            var spyTask = WebSocketFeed.SubscribeToLevel2Channel("XTZ-EUR").Spy(_testOutput, TimeSpan.FromMilliseconds(100));
            await WebSocketFeed.StartAsync();

            var spy = await spyTask;
            webSocketMock.Verify(socket => socket.ConnectAsync(It.Is<Uri>(u => u == EndpointUriNames.WebsocketFeedUri), It.IsAny<CancellationToken>()), Times.Exactly(1));
            webSocketMock.Verify(socket => socket.SendAsync(It.Is<ReadOnlyMemory<byte>>(buffer => buffer.ContainsUtf8String("\"level2\",\"product_ids\":[\"XTZ-EUR\"]")), It.Is<WebSocketMessageType>(t => t == WebSocketMessageType.Text), It.Is<bool>(j => j), It.IsAny<CancellationToken>()), Times.Exactly(1));
            spy.ReceivedMessages.OfType<Level2SnapshotMessage>().ShouldHaveSingleItem();
            spy.ReceivedMessages.OfType<Level2UpdateMessage>().ShouldNotBeEmpty();
        }

        private static IWebSocketFeed CreateWebSocketFeed(IMock<IClientWebSocketFacade> webSocketMock)
        {
            var services = new ServiceCollection();

            var serviceProvider = services
                .AddLogging()
                .AddTransient(_ => webSocketMock.Object)
                .AddCoinbaseProApi()
                .BuildServiceProvider();

            return serviceProvider.GetRequiredService<IWebSocketFeed>();
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            if (WebSocketFeed != null)
            {
                await WebSocketFeed.StopAsync();
            }
        }
    }
}
