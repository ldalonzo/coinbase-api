using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace LDZ.Coinbase.Api.Net.WebSockets
{
    public interface IClientWebSocketFacade : IDisposable
    {
        Task ConnectAsync(Uri uri, CancellationToken cancellationToken);

        ValueTask SendAsync(ReadOnlyMemory<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken);

        ValueTask<ValueWebSocketReceiveResult> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken);
    }
}
