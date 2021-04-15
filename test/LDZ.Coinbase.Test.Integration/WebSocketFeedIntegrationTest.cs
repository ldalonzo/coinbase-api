using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LDZ.Coinbase.Api;
using LDZ.Coinbase.Api.Model.Feed;
using LDZ.Coinbase.Api.Options;
using LDZ.Coinbase.Test.Shared;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace LDZ.Coinbase.Test.Integration
{
    public class WebSocketFeedIntegrationTest : IAsyncLifetime
    {
        public WebSocketFeedIntegrationTest(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;

            Factory = CoinbaseApiFactory.Create(configureOptions: options => options.UseSandbox());
            WebSocketFeed = Factory.CreateWebSocketFeed();
        }

        private readonly ITestOutputHelper _testOutput;

        private CoinbaseApiFactory Factory { get; }
        private IWebSocketFeed WebSocketFeed { get; }

        [Theory]
        [InlineData("BTC-USD")]
        public async Task SubscribeToHeartbeatChannel(string productId)
        {
            var spyTask = WebSocketFeed.SubscribeToHeartbeatChannel(productId).Spy(_testOutput, TimeSpan.FromSeconds(3));

            await WebSocketFeed.StartAsync();
            var spy = await spyTask;

            spy.ReceivedMessages.ShouldNotBeEmpty();
            var actualMessages = spy.ReceivedMessages.OfType<HeartbeatMessage>().ToList();
            actualMessages.ShouldNotBeEmpty();
            actualMessages.ShouldContain(m => m.ProductId == productId);
        }

        [Theory]
        [InlineData("BTC-USD", "BTC-EUR")]
        public async Task SubscribeToMultipleHeartbeatChannels(string productId1, string productId2)
        {
            var timeoutSource = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            var spyTask1 = WebSocketFeed.SubscribeToHeartbeatChannel(productId1).Spy(_testOutput, timeoutSource.Token);
            var spyTask2 = WebSocketFeed.SubscribeToHeartbeatChannel(productId2).Spy(_testOutput, timeoutSource.Token);

            await WebSocketFeed.StartAsync(CancellationToken.None);
            var spy1 = await spyTask1;
            var spy2 = await spyTask2;

            spy1.ReceivedMessages.OfType<HeartbeatMessage>().ShouldContain(m => m.ProductId == productId1);
            spy2.ReceivedMessages.OfType<HeartbeatMessage>().ShouldContain(m => m.ProductId == productId2);
        }

        [Theory]
        [InlineData("BTC-USD")]
        public async Task SubscribeToTickerChannel(string productId)
        {
            var spyTask = WebSocketFeed.SubscribeToTickerChannel(productId).Spy(_testOutput, TimeSpan.FromSeconds(3));

            await WebSocketFeed.StartAsync();
            var spy = await spyTask;

            spy.ReceivedMessages.ShouldNotBeEmpty();
            var actualMessages = spy.ReceivedMessages.OfType<TickerMessage>().ToList();
            actualMessages.ShouldNotBeEmpty();
            actualMessages.ShouldContain(m => m.ProductId == productId);
        }

        [Theory]
        [InlineData("BTC-USD")]
        public async Task SubscribeToLevel2Channel(string productId)
        {
            var spyTask = WebSocketFeed.SubscribeToLevel2Channel(productId).Spy(_testOutput, TimeSpan.FromSeconds(3));

            await WebSocketFeed.StartAsync();
            var spy = await spyTask;

            spy.ReceivedMessages.ShouldNotBeEmpty();
            var snapshotMessage = spy.ReceivedMessages.OfType<Level2SnapshotMessage>().ShouldHaveSingleItem();
            snapshotMessage.ShouldNotBeNull();
            snapshotMessage.ProductId.ShouldBe(productId);
            var updateMessages = spy.ReceivedMessages.OfType<Level2UpdateMessage>().ToList();
            updateMessages.ShouldNotBeEmpty();
            updateMessages.ShouldContain(m => m.ProductId == productId);
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            await WebSocketFeed.StopAsync();
            Factory.Dispose();

            _testOutput.WriteLine("Disposed.");
        }
    }
}
