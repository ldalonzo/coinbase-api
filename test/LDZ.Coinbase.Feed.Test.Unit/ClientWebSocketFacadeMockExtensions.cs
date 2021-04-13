using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using LDZ.Coinbase.Api.Net.WebSockets;
using Moq;

namespace LDZ.Coinbase.Feed.Test.Unit
{
    public static class ClientWebSocketFacadeMockExtensions
    {
        public static ISetupSequentialWebSocketResult SetupReceiveAsyncSequence(this Mock<IClientWebSocketFacade> mock)
        {
            var sequentialSetup = new SetupSequentialWebSocketResult();

            var buffer = Memory<byte>.Empty;
            var cancellationToken = CancellationToken.None;

            mock
                .Setup(facade => facade.ReceiveAsync(It.IsAny<Memory<byte>>(), It.IsAny<CancellationToken>()))
                .Callback<Memory<byte>, CancellationToken>((b, c) => { buffer = b; cancellationToken = c; })
                .Returns(() => sequentialSetup.GetNextMessage(buffer, cancellationToken));

            return sequentialSetup;
        }

        private class SetupSequentialWebSocketResult : ISetupSequentialWebSocketResult
        {
            private readonly ConcurrentQueue<string> _messageQueue = new ConcurrentQueue<string>();

            public ISetupSequentialWebSocketResult Returns(string messagePath)
            {
                _messageQueue.Enqueue(messagePath);

                return this;
            }

            public async ValueTask<ValueWebSocketReceiveResult> GetNextMessage(Memory<byte> buffer, CancellationToken cancellationToken)
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (_messageQueue.TryDequeue(out var path))
                    {
                        var array = await File.ReadAllBytesAsync(path, cancellationToken).ConfigureAwait(false);
                        new Span<byte>(array, 0, array.Length).CopyTo(buffer.Span);
                        return new ValueWebSocketReceiveResult(array.Length, WebSocketMessageType.Binary, true);
                    }

                    await Task.Delay(250, cancellationToken).ConfigureAwait(false);
                }

                throw new TaskCanceledException();
            }
        }
    }
}
