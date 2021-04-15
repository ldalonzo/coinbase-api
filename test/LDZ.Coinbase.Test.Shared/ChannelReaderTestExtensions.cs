using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using LDZ.Coinbase.Api.Model.Feed;
using Xunit.Abstractions;

namespace LDZ.Coinbase.Test.Shared
{
    public static class ChannelReaderTestExtensions
    {
        public static Task<ReceivedMessageSpy> Spy<T>(this ChannelReader<T> reader, ITestOutputHelper outputHelper, TimeSpan timeout)
            where T : FeedResponseMessage
            => reader.Spy(outputHelper, new CancellationTokenSource(timeout).Token);

        public static async Task<ReceivedMessageSpy> Spy<T>(this ChannelReader<T> reader, ITestOutputHelper outputHelper, CancellationToken cancellationToken)
            where T : FeedResponseMessage
        {
            var spy = new ReceivedMessageSpy(outputHelper);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    spy.ReceiveMessage(await reader.ReadAsync(cancellationToken));
                }
                catch (OperationCanceledException)
                {
                    continue;
                }
            }

            return spy;
        }
    }
}
