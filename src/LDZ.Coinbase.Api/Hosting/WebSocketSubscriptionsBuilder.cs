using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using LDZ.Coinbase.Api.Model.Feed;
using LDZ.Coinbase.Api.Model.Feed.Channels;

namespace LDZ.Coinbase.Api.Hosting
{
    internal class WebSocketSubscriptionsBuilder
    {
        private readonly Dictionary<Type, Func<FeedResponseMessage, CancellationToken, ValueTask>> _handlersByMessageType = new Dictionary<Type, Func<FeedResponseMessage, CancellationToken, ValueTask>>();

        private HeartbeatChannel? _heartbeatChannel;
        private TickerChannel? _tickerChannel;
        private Level2Channel? _level2Channel;

        public ChannelReader<HeartbeatMessage> SubscribeToHeartbeatChannel(params string[] productIds)
        {
            _heartbeatChannel ??= new HeartbeatChannel();

            foreach (var productId in productIds)
            {
                _heartbeatChannel.Products.Add(productId);
            }

            return AddMessageHandler<HeartbeatMessage>();
        }

        public ChannelReader<TickerMessage> SubscribeToTickerChannel(params string[] productIds)
        {
            _tickerChannel ??= new TickerChannel();

            foreach (var productId in productIds)
            {
                _tickerChannel.Products.Add(productId);
            }

            return AddMessageHandler<TickerMessage>();
        }

        public ChannelReader<Level2Message> SubscribeToLevel2Channel(params string[] productIds)
        {
            _level2Channel ??= new Level2Channel();

            foreach (var productId in productIds)
            {
                _level2Channel.Products.Add(productId);
            }

            var channel = Channel.CreateUnbounded<Level2Message>();

            AddMessageHandler<Level2Message, Level2SnapshotMessage>(channel);
            AddMessageHandler<Level2Message, Level2UpdateMessage>(channel);

            return channel.Reader;
        }

        private void AddMessageHandler<TCMessage, TMessage>(Channel<TCMessage> channel)
            where TCMessage : FeedResponseMessage
            where TMessage : TCMessage
        {
            _handlersByMessageType.Add(typeof(TMessage), (message, cancellationToken) => channel.Writer.WriteAsync((TCMessage) message, cancellationToken));
        }

        private ChannelReader<TMessage> AddMessageHandler<TMessage>() where TMessage : FeedResponseMessage
        {
            var channel = Channel.CreateUnbounded<TMessage>();

            AddMessageHandler<TMessage, TMessage>(channel);

            return channel.Reader;
        }

        public IReadOnlyDictionary<Type, Func<FeedResponseMessage, CancellationToken, ValueTask>> BuildHandlers()
        {
            return _handlersByMessageType;
        }

        public IReadOnlyCollection<FeedChannel> BuildSubscriptions()
            => EnumerateSubscriptions().ToList();

        private IEnumerable<FeedChannel> EnumerateSubscriptions()
        {
            if (_heartbeatChannel != null)
            {
                yield return _heartbeatChannel;
            }

            if (_level2Channel != null)
            {
                yield return _level2Channel;
            }

            if (_tickerChannel != null)
            {
                yield return _tickerChannel;
            }
        }
    }
}
