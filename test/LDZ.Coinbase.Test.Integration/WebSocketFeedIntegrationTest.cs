using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LDZ.Coinbase.Api;
using LDZ.Coinbase.Api.Hosting;
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
        }

        private readonly ITestOutputHelper _testOutput;

        [Theory]
        [InlineData("BTC-USD")]
        public async Task SubscribeToHeartbeatChannel(string productId)
        {
            var spy = new ReceivedMessageSpy(_testOutput);

            await RecordMessages(r => r.SubscribeToHeartbeatChannel(spy.ReceiveMessage, productId));

            spy.ReceivedMessages.ShouldNotBeEmpty();
            var actualMessages = spy.ReceivedMessages.OfType<HeartbeatMessage>().ToList();
            actualMessages.ShouldNotBeEmpty();
            actualMessages.ShouldContain(m => m.ProductId == productId);
        }

        [Theory]
        [InlineData("BTC-USD")]
        public async Task SubscribeToTickerChannel(string productId)
        {
            var spy = new ReceivedMessageSpy(_testOutput);

            await RecordMessages(r => r.SubscribeToTickerChannel(spy.ReceiveMessage, productId));

            spy.ReceivedMessages.ShouldNotBeEmpty();
            var actualMessages = spy.ReceivedMessages.OfType<TickerMessage>().ToList();
            actualMessages.ShouldNotBeEmpty();
            actualMessages.ShouldContain(m => m.ProductId == productId);
        }

        [Theory]
        [InlineData("BTC-USD")]
        public async Task SubscribeToLevel2Channel(string productId)
        {
            var spy = new ReceivedMessageSpy(_testOutput);

            await RecordMessages(r => r.SubscribeToLevel2Channel(spy.ReceiveMessage, spy.ReceiveMessage, productId));

            spy.ReceivedMessages.ShouldNotBeEmpty();

            var snapshotMessage = spy.ReceivedMessages.OfType<Level2SnapshotMessage>().ShouldHaveSingleItem();
            snapshotMessage.ShouldNotBeNull();
            snapshotMessage.ProductId.ShouldBe(productId);

            var updateMessages = spy.ReceivedMessages.OfType<Level2UpdateMessage>().ToList();
            updateMessages.ShouldNotBeEmpty();
            updateMessages.ShouldContain(m => m.ProductId == productId);
        }

        private async Task RecordMessages(Action<IWebSocketSubscriptionsBuilder> configureFeed)
        {
            using var factory = CoinbaseApiFactory.Create(
                builder => builder.ConfigureFeed(configureFeed),
                options => options.UseSandbox());

            var feed = await factory.StartMarketDataFeed(CancellationToken.None);
            _testOutput.WriteLine("Started recording.");

            // Wait for some messages to come through.
            await Task.Delay(TimeSpan.FromSeconds(3));

            await feed.StopAsync(CancellationToken.None);
            _testOutput.WriteLine("Stopped recording.");
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            _testOutput.WriteLine("Disposed.");
            return Task.CompletedTask;
        }
    }
}
