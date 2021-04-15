using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using LDZ.Coinbase.Api.Model.Feed;
using LDZ.Coinbase.Api.Model.Feed.Channels;
using LDZ.Coinbase.Api.Net.WebSockets;
using LDZ.Coinbase.Api.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LDZ.Coinbase.Api.Hosting
{
    internal class WebSocketFeed : IWebSocketFeed
    {
        public WebSocketFeed(
            ILogger<WebSocketFeed> log,
            IOptions<CoinbaseApiOptions> apiOptions,
            IOptions<JsonSerializerOptions> serializerOptions,
            IClientWebSocketFacade webSocket)
        {
            _log = log;
            _apiOptions = apiOptions.Value;
            _serializerOptions = serializerOptions.Value;
            _webSocket = webSocket;
        }

        private readonly ILogger _log;

        private readonly CoinbaseApiOptions _apiOptions;
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly IClientWebSocketFacade _webSocket;

        private readonly Channel<FeedResponseMessage> _channel = Channel.CreateUnbounded<FeedResponseMessage>();

        public ChannelReader<FeedResponseMessage> ChannelReader => _channel.Reader;

        private CancellationTokenSource? _cancellationTokenSource;
        private Task? _receiveAllMessagesTask;

        public async Task Subscribe(Action<IWebSocketSubscriptionsBuilder> configure, CancellationToken cancellationToken = default)
        {
            var builder = new WebSocketSubscriptionsBuilder();
            configure(builder);

            await SendSubscribeMessage(builder);

            if (_cancellationTokenSource == null)
            {
                _cancellationTokenSource = new CancellationTokenSource();
                _receiveAllMessagesTask = ReceiveAllMessagesAsync(_channel.Writer, _cancellationTokenSource.Token);
            }
        }

        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            if (_apiOptions.WebsocketFeedUri == null)
            {
                throw new InvalidOperationException("Websocket Feed URL is not specified.");
            }

            try
            {
                await _webSocket.ConnectAsync(_apiOptions.WebsocketFeedUri, cancellationToken);
                _log.LogInformation($"Connected to {_apiOptions.WebsocketFeedUri}");
            }
            catch (WebSocketException ex)
            {
                _log.LogError($"Failed to connect. {ex.Message}");
                throw;
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            _log.LogInformation("Stopping.");

            _cancellationTokenSource?.Cancel();

            if (_receiveAllMessagesTask != null)
            {
                await _receiveAllMessagesTask;
            }

            _log.LogInformation("Stopped.");
        }

        private Task SendSubscribeMessage(WebSocketSubscriptionsBuilder builder)
            => SendSubscribeMessage(builder.BuildSubscriptions().ToArray());
       
        private async Task SendSubscribeMessage(params FeedChannel[] channels)
        {
            var message = SubscribeMessage.Create(channels);
            var bytes = JsonSerializer.SerializeToUtf8Bytes<FeedRequestMessage>(message, _serializerOptions);
            await _webSocket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private async Task ReceiveAllMessagesAsync(ChannelWriter<FeedResponseMessage> writer, CancellationToken cancellationToken)
        {
            try
            {
                await foreach (var message in ReceiveAllMessagesAsync(cancellationToken))
                {
                    await writer.WriteAsync(message, cancellationToken);
                }
            }
            catch (TaskCanceledException)
            {
                writer.Complete();
            }
        }

        private async IAsyncEnumerable<FeedResponseMessage> ReceiveAllMessagesAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                yield return await ReceiveMessageAsync(cancellationToken);
            }
        }

        private async Task<FeedResponseMessage> ReceiveMessageAsync(CancellationToken cancellationToken)
        {
            var buffer = new Memory<byte>(new byte[2048]);

            ValueWebSocketReceiveResult result;
            await using var stream = new MemoryStream();
            do
            {
                result = await _webSocket.ReceiveAsync(buffer, cancellationToken);
                await stream.WriteAsync(buffer.Slice(0, result.Count), cancellationToken);
            } while (!result.EndOfMessage);

            stream.Seek(0, SeekOrigin.Begin);

            var message = await JsonSerializer.DeserializeAsync<FeedResponseMessage>(stream, _serializerOptions, cancellationToken);
            if (message == null)
            {
                throw new InvalidOperationException();
            }

            return message;
        }
    }
}
