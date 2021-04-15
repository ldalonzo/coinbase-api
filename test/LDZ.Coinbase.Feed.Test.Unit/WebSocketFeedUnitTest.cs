using System;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
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
    public class WebSocketFeedUnitTest
    {
        public WebSocketFeedUnitTest(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        private readonly ITestOutputHelper _outputHelper;

        [Fact]
        public async Task ConnectAsync_MissingWebsocketFeedUri()
        {
            var services = new ServiceCollection();
            await using var serviceProvider = services
                .AddCoinbaseProApi(_ => { }, c => c.Configure(r => r.WebsocketFeedUri = null))
                .BuildServiceProvider();

            var webSocketFeed = serviceProvider.GetRequiredService<IWebSocketFeed>();

            var actualErrorMessage = await Should.ThrowAsync<InvalidOperationException>(() => webSocketFeed.ConnectAsync());
            actualErrorMessage.Message.ShouldContain("Websocket Feed URL is not specified.");
        }

        [Fact]
        public async Task ConnectAsync()
        {
            var webSocketMock = new Mock<IClientWebSocketFacade>();

            var services = new ServiceCollection();
            await using var serviceProvider = services
                .AddTransient(_ => webSocketMock.Object)
                .AddCoinbaseProApi()
                .BuildServiceProvider();

            var webSocketFeed = serviceProvider.GetRequiredService<IWebSocketFeed>();

            await webSocketFeed.ConnectAsync();

            webSocketMock.Verify(socket => socket.ConnectAsync(
                    It.Is<Uri>(u => u == EndpointUriNames.WebsocketFeedUri),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(1));
        }

        [Fact]
        public async Task ConnectAsyncThrowsWebSocketException()
        {
            var webSocketMock = new Mock<IClientWebSocketFacade>();
            webSocketMock
                .Setup(f => f.ConnectAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
                .Throws<WebSocketException>();

            var services = new ServiceCollection();
            await using var serviceProvider = services
                .AddTransient(_ => webSocketMock.Object)
                .AddCoinbaseProApi()
                .BuildServiceProvider();

            var webSocketFeed = serviceProvider.GetRequiredService<IWebSocketFeed>();

            await Should.ThrowAsync<WebSocketException>(webSocketFeed.ConnectAsync());
        }

        [Theory]
        [AutoData]
        public async Task SubscribeToHeartbeatChannel(string productId)
        {
            var webSocketMock = new Mock<IClientWebSocketFacade>();
            webSocketMock
                .Setup(f => f.ReceiveAsync(It.IsAny<Memory<byte>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValueWebSocketReceiveResult(0, WebSocketMessageType.Text, true));

            var services = new ServiceCollection();
            await using var serviceProvider = services
                .AddTransient(_ => webSocketMock.Object)
                .AddCoinbaseProApi()
                .BuildServiceProvider();

            var webSocketFeed = serviceProvider.GetRequiredService<IWebSocketFeed>();
            await webSocketFeed.Subscribe(b => b.AddHeartbeatChannel(productId));

            webSocketMock.Verify(socket => socket.SendAsync(
                    It.Is<ReadOnlyMemory<byte>>(buffer => buffer.ContainsUtf8String($"\"heartbeat\",\"product_ids\":[\"{productId}\"]")),
                    It.Is<WebSocketMessageType>(t => t == WebSocketMessageType.Text),
                    It.Is<bool>(b => b), It.IsAny<CancellationToken>()),
                Times.Exactly(1));
        }

        [Theory]
        [AutoData]
        public async Task SubscribeToTickerChannel(string productId)
        {
            var webSocketMock = new Mock<IClientWebSocketFacade>();
            webSocketMock
                .Setup(f => f.ReceiveAsync(It.IsAny<Memory<byte>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValueWebSocketReceiveResult(0, WebSocketMessageType.Text, true));

            var services = new ServiceCollection();
            await using var serviceProvider = services
                .AddTransient(_ => webSocketMock.Object)
                .AddCoinbaseProApi()
                .BuildServiceProvider();

            var webSocketFeed = serviceProvider.GetRequiredService<IWebSocketFeed>();
            await webSocketFeed.Subscribe(b => b.AddTickerChannel(productId));

            webSocketMock.Verify(socket => socket.SendAsync(
                    It.Is<ReadOnlyMemory<byte>>(buffer => buffer.ContainsUtf8String($"\"ticker\",\"product_ids\":[\"{productId}\"]")),
                    It.Is<WebSocketMessageType>(t => t == WebSocketMessageType.Text),
                    It.Is<bool>(j => j), It.IsAny<CancellationToken>()),
                Times.Exactly(1));
        }

        [Theory]
        [AutoData]
        public async Task SubscribeToLevel2Channel(string productId)
        {
            var webSocketMock = new Mock<IClientWebSocketFacade>();
            webSocketMock
                .Setup(f => f.ReceiveAsync(It.IsAny<Memory<byte>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValueWebSocketReceiveResult(0, WebSocketMessageType.Text, true));

            var services = new ServiceCollection();
            await using var serviceProvider = services
                .AddTransient(_ => webSocketMock.Object)
                .AddCoinbaseProApi()
                .BuildServiceProvider();

            var webSocketFeed = serviceProvider.GetRequiredService<IWebSocketFeed>();
            await webSocketFeed.Subscribe(b => b.AddLevel2Channel(productId));

            webSocketMock.Verify(socket => socket.SendAsync(
                    It.Is<ReadOnlyMemory<byte>>(buffer => buffer.ContainsUtf8String($"\"level2\",\"product_ids\":[\"{productId}\"]")),
                    It.Is<WebSocketMessageType>(t => t == WebSocketMessageType.Text),
                    It.Is<bool>(j => j), It.IsAny<CancellationToken>()),
                Times.Exactly(1));
        }

        [Fact]
        public async Task ReceiveHeartbeatMessages()
        {
            var webSocketMock = new Mock<IClientWebSocketFacade>();
            webSocketMock
                .SetupReceiveAsyncSequence()
                .Returns("TestData/message_BTC-USD_heartbeat.json");

            var services = new ServiceCollection();
            await using var serviceProvider = services
                .AddTransient(_ => webSocketMock.Object)
                .AddCoinbaseProApi()
                .BuildServiceProvider();

            var webSocketFeed = serviceProvider.GetRequiredService<IWebSocketFeed>();
            await webSocketFeed.Subscribe(_ => { });
            
            var spy = await webSocketFeed.ChannelReader.Spy(_outputHelper, TimeSpan.FromMilliseconds(100));
            spy.ReceivedMessages.OfType<HeartbeatMessage>().ShouldNotBeEmpty();
        }

        [Fact]
        public async Task ReceiveTickerMessages()
        {
            var webSocketMock = new Mock<IClientWebSocketFacade>();
            webSocketMock
                .SetupReceiveAsyncSequence()
                .Returns("TestData/message_ETH-EUR_ticker.json");

            var services = new ServiceCollection();
            await using var serviceProvider = services
                .AddTransient(_ => webSocketMock.Object)
                .AddCoinbaseProApi()
                .BuildServiceProvider();

            var webSocketFeed = serviceProvider.GetRequiredService<IWebSocketFeed>();
            await webSocketFeed.Subscribe(_ => { });

            var spy = await webSocketFeed.ChannelReader.Spy(_outputHelper, TimeSpan.FromMilliseconds(100));
            spy.ReceivedMessages.OfType<TickerMessage>().ShouldNotBeEmpty();
        }

        [Fact]
        public async Task ReceiveLevel2Messages()
        {
            var webSocketMock = new Mock<IClientWebSocketFacade>();
            webSocketMock
                .SetupReceiveAsyncSequence()
                .Returns("TestData/message_XTZ-EUR_snapshot.json")
                .Returns("TestData/message_XTZ-EUR_l2update_buy.json")
                .Returns("TestData/message_XTZ-EUR_l2update_sell.json");

            var services = new ServiceCollection();
            await using var serviceProvider = services
                .AddTransient(_ => webSocketMock.Object)
                .AddCoinbaseProApi()
                .BuildServiceProvider();

            var webSocketFeed = serviceProvider.GetRequiredService<IWebSocketFeed>();
            await webSocketFeed.Subscribe(_ => { });

            var spy = await webSocketFeed.ChannelReader.Spy(_outputHelper, TimeSpan.FromMilliseconds(100));
            spy.ReceivedMessages.OfType<Level2SnapshotMessage>().ShouldHaveSingleItem();
            spy.ReceivedMessages.OfType<Level2UpdateMessage>().ShouldNotBeEmpty();
        }
    }
}
