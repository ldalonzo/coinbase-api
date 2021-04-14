using System;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using LDZ.Coinbase.Api;
using LDZ.Coinbase.Api.DependencyInjection;
using LDZ.Coinbase.Api.Hosting;
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
    public class WebSocketFeedUnitTest
    {
        private readonly ITestOutputHelper _outputHelper;

        public WebSocketFeedUnitTest(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact]
        public async Task SubscribeToHeartbeatChannel()
        {
            var spy = new ReceivedMessageSpy(_outputHelper);

            var webSocketMock = new Mock<IClientWebSocketFacade>();
            webSocketMock.SetupReceiveAsyncSequence()
                .Returns("TestData/message_BTC-USD_heartbeat.json");

            await RecordMessages(apiBuilder => apiBuilder
                .ConfigureFeed(builder => builder.SubscribeToHeartbeatChannel(spy.ReceiveMessage, "BTC-USD")), webSocketMock.Object);

            webSocketMock.Verify(socket => socket.ConnectAsync(It.Is<Uri>(u => u == EndpointUriNames.WebsocketFeedUri), It.IsAny<CancellationToken>()), Times.Exactly(1));
            webSocketMock.Verify(socket => socket.SendAsync(It.Is<ReadOnlyMemory<byte>>(buffer => buffer.ContainsUtf8String("\"heartbeat\",\"product_ids\":[\"BTC-USD\"]") ), It.Is<WebSocketMessageType>(t => t == WebSocketMessageType.Text), It.Is<bool>(b => b), It.IsAny<CancellationToken>()), Times.Exactly(1));

            spy.ReceivedMessages.OfType<HeartbeatMessage>().ShouldNotBeEmpty();
        }

        [Fact]
        public async Task SubscribeToTickerChannel()
        {
            var spy = new ReceivedMessageSpy(_outputHelper);
            var webSocketMock = new Mock<IClientWebSocketFacade>();
            webSocketMock.SetupReceiveAsyncSequence()
                .Returns("TestData/message_ETH-EUR_ticker.json");

            await RecordMessages(apiBuilder => apiBuilder
                .ConfigureFeed(builder => builder.SubscribeToTickerChannel(spy.ReceiveMessage, "ETH-EUR")), webSocketMock.Object);

            webSocketMock.Verify(socket => socket.ConnectAsync(It.Is<Uri>(u => u == EndpointUriNames.WebsocketFeedUri), It.IsAny<CancellationToken>()), Times.Exactly(1));
            webSocketMock.Verify(socket => socket.SendAsync(It.Is<ReadOnlyMemory<byte>>(buffer => buffer.ContainsUtf8String("\"ticker\",\"product_ids\":[\"ETH-EUR\"]")), It.Is<WebSocketMessageType>(t => t == WebSocketMessageType.Text), It.Is<bool>(j => j), It.IsAny<CancellationToken>()), Times.Exactly(1));
            spy.ReceivedMessages.OfType<TickerMessage>().ShouldHaveSingleItem();
        }

        [Fact]
        public async Task SubscribeToLevel2Channel()
        {
            var spy = new ReceivedMessageSpy(_outputHelper);
            var webSocketMock = new Mock<IClientWebSocketFacade>();
            webSocketMock
                .SetupReceiveAsyncSequence()
                .Returns("TestData/message_XTZ-EUR_snapshot.json")
                .Returns("TestData/message_XTZ-EUR_l2update_buy.json")
                .Returns("TestData/message_XTZ-EUR_l2update_sell.json");

            await RecordMessages(apiBuilder => apiBuilder
                .ConfigureFeed(builder => builder.SubscribeToLevel2Channel(spy.ReceiveMessage, spy.ReceiveMessage, "XTZ-EUR")), webSocketMock.Object);

            webSocketMock.Verify(socket => socket.ConnectAsync(It.Is<Uri>(u => u == EndpointUriNames.WebsocketFeedUri), It.IsAny<CancellationToken>()), Times.Exactly(1));
            webSocketMock.Verify(socket => socket.SendAsync(It.Is<ReadOnlyMemory<byte>>(buffer => buffer.ContainsUtf8String("\"level2\",\"product_ids\":[\"XTZ-EUR\"]")), It.Is<WebSocketMessageType>(t => t == WebSocketMessageType.Text), It.Is<bool>(j => j), It.IsAny<CancellationToken>()), Times.Exactly(1));
            spy.ReceivedMessages.OfType<Level2SnapshotMessage>().ShouldHaveSingleItem();
            spy.ReceivedMessages.OfType<Level2UpdateMessage>().ShouldNotBeEmpty();
        }

        private static async Task RecordMessages(Action<ICoinbaseApiBuilder>? configure, IClientWebSocketFacade webSocket)
        {
            var services = new ServiceCollection();

            await using var serviceProvider = services
                .AddLogging()
                .AddTransient(_ => webSocket)
                .AddCoinbaseProApi(configure)
                .BuildServiceProvider();

            var feed = serviceProvider.GetRequiredService<IMarketDataFeedMessagePublisher>();

            await feed.StartAsync().ConfigureAwait(false);
            await Task.Delay(500);
            await feed.StopAsync().ConfigureAwait(false);
        }
    }
}
