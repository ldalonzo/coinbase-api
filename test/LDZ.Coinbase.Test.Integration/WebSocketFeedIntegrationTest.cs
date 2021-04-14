using System;
using System.Linq;
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

            _factory = CoinbaseApiFactory.Create(configureOptions: options => options.UseSandbox());
            _webSocketFeed = _factory.CreateWebSocketFeed();
        }

        private readonly ITestOutputHelper _testOutput;

        private readonly CoinbaseApiFactory _factory;
        private readonly IMarketDataFeedMessagePublisher _webSocketFeed;

        [Theory]
        [InlineData("BTC-USD")]
        public async Task SubscribeToHeartbeatChannel(string productId)
        {
            var spy = await _webSocketFeed
                .SubscribeToHeartbeatChannel(productId)
                .RecordMessagesAsync(_webSocketFeed, _testOutput, TimeSpan.FromSeconds(3));

            spy.ReceivedMessages.ShouldNotBeEmpty();
            var actualMessages = spy.ReceivedMessages.OfType<HeartbeatMessage>().ToList();
            actualMessages.ShouldNotBeEmpty();
            actualMessages.ShouldContain(m => m.ProductId == productId);
        }

        [Theory]
        [InlineData("BTC-USD")]
        public async Task SubscribeToTickerChannel(string productId)
        {
            var spy = await _webSocketFeed
                .SubscribeToTickerChannel(productId)
                .RecordMessagesAsync(_webSocketFeed, _testOutput, TimeSpan.FromSeconds(3));

            spy.ReceivedMessages.ShouldNotBeEmpty();
            var actualMessages = spy.ReceivedMessages.OfType<TickerMessage>().ToList();
            actualMessages.ShouldNotBeEmpty();
            actualMessages.ShouldContain(m => m.ProductId == productId);
        }

        [Theory]
        [InlineData("BTC-USD")]
        public async Task SubscribeToLevel2Channel(string productId)
        {
            var spy = await _webSocketFeed
                .SubscribeToLevel2Channel(productId)
                .RecordMessagesAsync(_webSocketFeed, _testOutput, TimeSpan.FromSeconds(3));

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

        public Task DisposeAsync()
        {
            _factory.Dispose();

            _testOutput.WriteLine("Disposed.");
            return Task.CompletedTask;
        }
    }
}
