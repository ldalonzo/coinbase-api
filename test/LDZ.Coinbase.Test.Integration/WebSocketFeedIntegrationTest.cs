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
        }

        private readonly ITestOutputHelper _testOutput;

        [Theory]
        [InlineData("BTC-USD")]
        public async Task SubscribeToHeartbeatChannel(string productId)
        {
            var spy = new ReceivedMessageSpy(_testOutput);

            var factory = CoinbaseApiFactory.Create(
                builder => builder.ConfigureFeed(r => r.SubscribeToHeartbeatChannel(spy.ReceiveMessage, productId)),
                options => options.UseSandbox());

            var feed = await factory.StartMarketDataFeed(CancellationToken.None);
            _testOutput.WriteLine("Started recording.");

            // Wait for some messages to come through.
            await Task.Delay(TimeSpan.FromSeconds(3));

            await feed.StopAsync(CancellationToken.None);
            _testOutput.WriteLine("Stopped recording.");

            spy.ReceivedMessages.ShouldNotBeEmpty();
            var actualMessages = spy.ReceivedMessages.OfType<HeartbeatMessage>().ToList();
            actualMessages.ShouldNotBeEmpty();
            actualMessages.ShouldContain(m => m.ProductId == productId);
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
