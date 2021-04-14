using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace LDZ.Coinbase.Api.Net.WebSockets
{
    internal class ClientWebSocketFacade : IClientWebSocketFacade
    {
        public ClientWebSocketFacade()
        {
            _webSocket = new ClientWebSocket();
        }

        private readonly ClientWebSocket _webSocket;

        public Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
            => _webSocket.ConnectAsync(uri, cancellationToken);

        public ValueTask SendAsync(ReadOnlyMemory<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
            => _webSocket.SendAsync(buffer, messageType, endOfMessage, cancellationToken);

        public ValueTask<ValueWebSocketReceiveResult> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken)
            => _webSocket.ReceiveAsync(buffer, cancellationToken);

        public void Dispose()
        {
            _webSocket.Dispose();
        }
    }
}
