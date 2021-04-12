using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using LDZ.Coinbase.Api.Net.WebSockets;

namespace LDZ.Coinbase.Feed.Test.Unit
{
    public class ClientWebSocketMock : IClientWebSocketFacade
    {
        private readonly BlockingCollection<string> _messagesQueue = new BlockingCollection<string>(new ConcurrentQueue<string>());

        public void SetupReceiveMessage(string path, CancellationToken cancellationToken = default)
        {
            _messagesQueue.Add(path, cancellationToken);
        }

        public Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async ValueTask SendAsync(ReadOnlyMemory<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
        {
            await Task.Yield();
        }

        public async ValueTask<ValueWebSocketReceiveResult> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken)
        {
            await Task.Yield();

            var path = _messagesQueue.Take(cancellationToken);
            var array = await File.ReadAllBytesAsync(path, cancellationToken);

            new Span<byte>(array, 0, array.Length).CopyTo(buffer.Span);

            return new ValueWebSocketReceiveResult(array.Length, WebSocketMessageType.Binary, true);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
