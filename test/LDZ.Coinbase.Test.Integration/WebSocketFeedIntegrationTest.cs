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

            Factory = CoinbaseApiFactory.Create(configureOptions: options => options.UseProduction());
            WebSocketFeed = Factory.CreateWebSocketFeed();
        }

        private readonly ITestOutputHelper _testOutput;

        private CoinbaseApiFactory Factory { get; }
        private IWebSocketFeed WebSocketFeed { get; }

        [Theory]
        [InlineData("BTC-USD")]
        public async Task SubscribeToHeartbeatChannel(string productId)
        {
            await WebSocketFeed.Subscribe(b => b.AddHeartbeatChannel(productId));
            
            var spy = await WebSocketFeed.ChannelReader.Spy(_testOutput, TimeSpan.FromSeconds(3));

            spy.ReceivedMessages.ShouldNotBeEmpty();
            var actualMessages = spy.ReceivedMessages.OfType<HeartbeatMessage>().ToList();
            actualMessages.ShouldNotBeEmpty();
            actualMessages.ShouldContain(m => m.ProductId == productId);
        }

        [Theory]
        [InlineData("BTC-USD")]
        public async Task SubscribeToTickerChannel(string productId)
        {
            await WebSocketFeed.Subscribe(b => b.AddTickerChannel(productId));

            var spy = await WebSocketFeed.ChannelReader.Spy(_testOutput, TimeSpan.FromSeconds(3));

            spy.ReceivedMessages.ShouldNotBeEmpty();
            var actualMessages = spy.ReceivedMessages.OfType<TickerMessage>().ToList();
            actualMessages.ShouldNotBeEmpty();
            actualMessages.ShouldContain(m => m.ProductId == productId);
        }

        [Theory]
        [InlineData("BTC-USD")]
        public async Task SubscribeToLevel2Channel(string productId)
        {
            await WebSocketFeed.Subscribe(b => b.AddLevel2Channel(productId));
            var spy = await WebSocketFeed.ChannelReader.Spy(_testOutput, TimeSpan.FromSeconds(3));

            spy.ReceivedMessages.ShouldNotBeEmpty();
            var snapshotMessage = spy.ReceivedMessages.OfType<Level2SnapshotMessage>().ShouldHaveSingleItem();
            snapshotMessage.ShouldNotBeNull();
            snapshotMessage.ProductId.ShouldBe(productId);
            var updateMessages = spy.ReceivedMessages.OfType<Level2UpdateMessage>().ToList();
            updateMessages.ShouldNotBeEmpty();
            updateMessages.ShouldContain(m => m.ProductId == productId);
        }

        public async Task InitializeAsync()
        {
            await WebSocketFeed.ConnectAsync();

            _testOutput.WriteLine("Connected.");
        }

        public async Task DisposeAsync()
        {
            await WebSocketFeed.StopAsync();
            Factory.Dispose();

            _testOutput.WriteLine("Disposed.");
        }
    }
}
