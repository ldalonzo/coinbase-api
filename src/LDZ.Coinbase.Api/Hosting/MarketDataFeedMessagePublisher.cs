using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
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
    internal class MarketDataFeedMessagePublisher : IMarketDataFeedMessagePublisher
    {
        public MarketDataFeedMessagePublisher(
            ILogger<MarketDataFeedMessagePublisher> log,
            IOptions<CoinbaseApiOptions> apiOptions,
            IOptions<JsonSerializerOptions> serializerOptions,
            IClientWebSocketFacade webSocket)
        {
            _log = log;
            _apiOptions = apiOptions.Value;
            _serializerOptions = serializerOptions.Value;
            _webSocket = webSocket;

            var channel = Channel.CreateUnbounded<FeedResponseMessage>();
            _reader = channel.Reader;
            _writer = channel.Writer;
        }

        private readonly ILogger _log;

        private readonly CoinbaseApiOptions _apiOptions;
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly IClientWebSocketFacade _webSocket;

        private readonly WebSocketSubscriptionsBuilder _subscriptionsBuilder = new WebSocketSubscriptionsBuilder();

        private readonly ChannelReader<FeedResponseMessage> _reader;
        private readonly ChannelWriter<FeedResponseMessage> _writer;

        private IReadOnlyDictionary<Type, Func<FeedResponseMessage, CancellationToken, ValueTask>>? _handlersByMessageType;

        private CancellationTokenSource? _cancellationTokenSource;
        private Task? _producerConsumerTask;

        public ChannelReader<HeartbeatMessage> SubscribeToHeartbeatChannel(params string[] productIds)
        {
            return _subscriptionsBuilder.SubscribeToHeartbeatChannel(productIds);
        }

        public ChannelReader<TickerMessage> SubscribeToTickerChannel(params string[] productIds)
        {
            return _subscriptionsBuilder.SubscribeToTickerChannel(productIds);
        }

        public ChannelReader<Level2Message> SubscribeToLevel2Channel(params string[] productIds)
        {
            return _subscriptionsBuilder.SubscribeToLevel2Channel(productIds);
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            _handlersByMessageType = _subscriptionsBuilder.BuildHandlers();

            await SubscribeAsync(_subscriptionsBuilder.BuildSubscriptions(), cancellationToken);

            _cancellationTokenSource = new CancellationTokenSource();

            _producerConsumerTask = Task.WhenAll(
                ReceiveMessagesAsync(_cancellationTokenSource.Token),
                ConsumeMessagesAsync(_cancellationTokenSource.Token));

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

        private async Task ConsumeMessagesAsync(CancellationToken cancellationToken)
        {
            while (await _reader.WaitToReadAsync(cancellationToken))
            {
                var trade = await _reader.ReadAsync(cancellationToken);

                if (_handlersByMessageType == null || !_handlersByMessageType.TryGetValue(trade.GetType(), out var handle))
                {
                    _log.LogInformation($"Drop {trade}");
                    continue;
                }

                _log.LogInformation($"Dispatch {trade}");

                try
                {
                    await handle(trade, cancellationToken);
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, ex.Message);
                }
            }

            _log.LogInformation("Stopped consuming messages.");
        }

        private async Task ReceiveMessagesAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var message = await ReceiveMessageAsync(cancellationToken);

                    _log.LogTrace($"Enqueue {message}");
                    await _writer.WriteAsync(message, cancellationToken);
                }
                catch (TaskCanceledException ex)
                {
                    _log.LogDebug(ex.Message);
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, ex.Message);
                }
            }

            _writer.Complete();

            _log.LogInformation($"Stopped receiving messages.");
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
