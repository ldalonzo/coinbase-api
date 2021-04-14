using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using LDZ.Coinbase.Api;
using LDZ.Coinbase.Api.Model.Feed;
using Xunit.Abstractions;

namespace LDZ.Coinbase.Test.Shared
{
    public static class WebSocketFeedTestExtensions
    {
        public static async Task<ReceivedMessageSpy> RecordMessagesAsync<TMessage>(this ChannelReader<TMessage> reader, IMarketDataFeedMessagePublisher webSocketFeed, ITestOutputHelper testOutput, TimeSpan timeout)
            where TMessage : FeedResponseMessage
        {
            var spy = new ReceivedMessageSpy(testOutput);

            await webSocketFeed.StartAsync();
            await Task.WhenAny(Task.Delay(timeout), spy.ReceiveFromChannelAsync(reader));
            await webSocketFeed.StopAsync();

            return spy;
        }
    }
}
