using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using LDZ.Coinbase.Api.Model.Feed;
using LDZ.Coinbase.Api.Threading.Channels;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using Xunit.Sdk;

namespace LDZ.Coinbase.Test.Unit
{
    public class ChannelDemuxUnitTest
    {
        [Fact]
        public async Task CloseGracefully()
        {
            var sut = new ChannelDemux<FeedResponseMessage>(NullLoggerFactory.Instance);
            var sink = sut.AddSink<FeedResponseMessage>();

            // Close the source channel.
            await sut.WriteAsync(Channel.CreateUnbounded<FeedResponseMessage>().Reader, new CancellationTokenSource(TimeSpan.FromMilliseconds(50)).Token);

            try
            {
                // All the demux channels should close gracefully.
                await sink.WaitToReadAsync(new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token);
            }
            catch (OperationCanceledException)
            {
                throw new XunitException();
            }
        }
    }
}