using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LDZ.Coinbase.Api.Model.Feed;
using LDZ.Coinbase.Api.Model.Feed.Channels;
using LDZ.Coinbase.Api.Net.WebSockets;
using LDZ.Coinbase.Api.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LDZ.Coinbase.Api.Hosting
{
    internal class MarketDataFeedMessagePublisher : IMarketDataFeedMessagePublisher, IHostedService
    {
        public MarketDataFeedMessagePublisher(
            ILogger<MarketDataFeedMessagePublisher> log,
            IOptions<CoinbaseApiOptions> apiOptions,
            IOptions<JsonSerializerOptions> serializerOptions,
            IOptions<MarketDataFeedMessagePublisherOptions> subscriptionOptions,
            IClientWebSocketFacade webSocket)
        {
            _log = log;
            _apiOptions = apiOptions.Value;
            _serializerOptions = serializerOptions.Value;
            _subscriptionOptions = subscriptionOptions.Value;
            _webSocket = webSocket;
        }

        private readonly ILogger _log;

        private readonly CoinbaseApiOptions _apiOptions;
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly MarketDataFeedMessagePublisherOptions _subscriptionOptions;
        private readonly IClientWebSocketFacade _webSocket;

        private readonly BlockingCollection<FeedResponseMessage> _messagesQueue = new BlockingCollection<FeedResponseMessage>(new ConcurrentQueue<FeedResponseMessage>(), 1024);

        private IReadOnlyDictionary<Type, Action<FeedResponseMessage>>? _handlersByMessageType;

        private CancellationTokenSource? _cancellationTokenSource;
        private Task? _producerConsumerTask;

        private bool _started;

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (_started)
            {
                return;
            }

            if (_subscriptionOptions.Subscriptions != null && _subscriptionOptions.Subscriptions.Any())
            {
                _handlersByMessageType = _subscriptionOptions.Handlers;

                await SubscribeAsync(_subscriptionOptions.Subscriptions, cancellationToken);

                _cancellationTokenSource = new CancellationTokenSource();

                _producerConsumerTask = Task.WhenAll(
                    ReceiveMessagesAsync(_cancellationTokenSource.Token),
                    ConsumeMessagesAsync(_cancellationTokenSource.Token));
            }

            _started = true;
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

        private async Task SubscribeAsync(IEnumerable<Channel> channels, CancellationToken cancellationToken)
        {
            if (_apiOptions.WebsocketFeedUri == null)
            {
                throw new InvalidOperationException();
            }

            await _webSocket.ConnectAsync(_apiOptions.WebsocketFeedUri, cancellationToken);
            _log.LogInformation($"connected to {_apiOptions.WebsocketFeedUri}");

            // To begin receiving feed messages, you must first send a subscribe message to the server indicating which channels and products
            // to receive. This message is mandatory - you will be disconnected if no subscribe has been received within 5 seconds.
            var message = new SubscribeMessage {Channels = channels.ToList()};
            var bytes = JsonSerializer.SerializeToUtf8Bytes<FeedRequestMessage>(message, _serializerOptions);
            await _webSocket.SendAsync(bytes, WebSocketMessageType.Text, true, cancellationToken);
        }

        private async Task ConsumeMessagesAsync(CancellationToken cancellationToken)
        {
            await Task.Yield();

            while (!cancellationToken.IsCancellationRequested)
            {
                FeedResponseMessage trade;

                try
                {
                    trade = _messagesQueue.Take(CancellationToken.None);
                }
                catch (InvalidOperationException ex)
                {
                    _log.LogTrace(ex, $"{ex.Message}");
                    continue;
                }

                if (_handlersByMessageType == null || !_handlersByMessageType.TryGetValue(trade.GetType(), out var handle))
                {
                    _log.LogInformation($"Drop {trade}");
                    continue;
                }

                _log.LogInformation($"Dispatch {trade}");

                try
                {
                    handle(trade);
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
                    _log.LogDebug($"Enqueue {message}");

                    _messagesQueue.Add(message, cancellationToken);
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

            _messagesQueue.CompleteAdding();

            _log.LogInformation($"Stopped receiving messages. {_messagesQueue.Count} messages still in queue.");
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
