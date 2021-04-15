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
using LDZ.Coinbase.Api.Threading.Channels;
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
            IClientWebSocketFacade webSocket,
            ChannelDemux<FeedResponseMessage> demux)
        {
            _log = log;
            _apiOptions = apiOptions.Value;
            _serializerOptions = serializerOptions.Value;
            _webSocket = webSocket;
            _demux = demux;
        }

        private readonly ILogger _log;

        private readonly CoinbaseApiOptions _apiOptions;
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly IClientWebSocketFacade _webSocket;
        private readonly ChannelDemux<FeedResponseMessage> _demux;

        private WebSocketSubscriptionsBuilder SubscriptionsBuilder { get; } = new WebSocketSubscriptionsBuilder();

        private CancellationTokenSource? _cancellationTokenSource;
        private Task? _producerConsumerTask;

        public ChannelReader<HeartbeatMessage> SubscribeToHeartbeatChannel(params string[] productIds)
        {
            SubscriptionsBuilder.SubscribeToHeartbeatChannel(productIds);
            return _demux.AddSink<HeartbeatMessage>();
        }

        public ChannelReader<TickerMessage> SubscribeToTickerChannel(params string[] productIds)
        {
            SubscriptionsBuilder.SubscribeToTickerChannel(productIds);
            return _demux.AddSink<TickerMessage>();
        }

        public ChannelReader<Level2Message> SubscribeToLevel2Channel(params string[] productIds)
        {
            SubscriptionsBuilder.SubscribeToLevel2Channel(productIds);
            return _demux.AddSink<Level2Message>();
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            await SubscribeAsync(SubscriptionsBuilder.BuildSubscriptions(), cancellationToken);

            _cancellationTokenSource = new CancellationTokenSource();

            var channel = Channel.CreateUnbounded<FeedResponseMessage>();

            _producerConsumerTask = Task.WhenAll(
                ReceiveAllMessagesAsync(channel.Writer, _cancellationTokenSource.Token),
                _demux.WriteAsync(channel.Reader, _cancellationTokenSource.Token));

            _log.LogInformation("Started.");
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            _log.LogInformation("Stopping.");

            _cancellationTokenSource?.Cancel();

            if (_producerConsumerTask != null)
            {
                try
                {
                    await _producerConsumerTask;
                }
                catch (Exception ex)
                {
                    _log.LogTrace(ex, ex.Message);
                }
            }

            _log.LogInformation("Stopped.");
        }

        private async Task SubscribeAsync(IEnumerable<FeedChannel> channels, CancellationToken cancellationToken)
        {
            if (_apiOptions.WebsocketFeedUri == null)
            {
                throw new InvalidOperationException();
            }

            await _webSocket.ConnectAsync(_apiOptions.WebsocketFeedUri, cancellationToken);
            _log.LogInformation($"connected to {_apiOptions.WebsocketFeedUri}");

            // To begin receiving feed messages, you must first send a subscribe message to the server indicating which channels and products
            // to receive. This message is mandatory - you will be disconnected if no subscribe has been received within 5 seconds.
            await SendSubscribeMessage(channels.ToArray());
        }

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
            var buffer = new ArraySegment<byte>(new byte[2048]);
            if (buffer.Array == null)
            {
                throw new InvalidOperationException();
            }

            ValueWebSocketReceiveResult result;
            await using var stream = new MemoryStream();
            do
            {
                result = await _webSocket.ReceiveAsync(buffer, cancellationToken);
                await stream.WriteAsync(buffer.Array.AsMemory(buffer.Offset, result.Count), cancellationToken);
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
