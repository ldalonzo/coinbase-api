using System;
using System.Collections.Generic;
using System.Linq;
using LDZ.Coinbase.Api.Model.Feed;
using LDZ.Coinbase.Api.Model.Feed.Channels;

namespace LDZ.Coinbase.Api.Hosting
{
    internal class WebSocketSubscriptionsBuilder : IWebSocketSubscriptionsBuilder
    {
        private readonly Dictionary<Type, Action<FeedResponseMessage>> _handlersByMessageType = new Dictionary<Type, Action<FeedResponseMessage>>();

        private HeartbeatChannel? _heartbeatChannel;
        private TickerChannel? _tickerChannel;

        public void SubscribeToHeartbeatChannel(Action<HeartbeatMessage> onReceived, params string[] productIds)
        {
            if (!productIds.Any())
            {
                return;
            }

            _heartbeatChannel ??= new HeartbeatChannel();
            _heartbeatChannel.Products ??= new List<string>();
            
            foreach (var productId in productIds)
            {
                _heartbeatChannel.Products.Add(productId);
            }

            AddMessageHandler(onReceived);
        }

        public void SubscribeToTickerChannel(Action<TickerMessage> onReceived, params string[] productIds)
        {
            if (!productIds.Any())
            {
                return;
            }

            _tickerChannel ??= new TickerChannel();
            _tickerChannel.Products ??= new List<string>();

            foreach (var productId in productIds)
            {
                _tickerChannel.Products.Add(productId);
            }

            AddMessageHandler(onReceived);
        }

        private void AddMessageHandler<TMessage>(Action<TMessage> onReceived) where TMessage : FeedResponseMessage
            =>_handlersByMessageType.Add(typeof(TMessage), r => onReceived((TMessage)r));

        public IReadOnlyDictionary<Type, Action<FeedResponseMessage>> BuildHandlers()
        {
            return _handlersByMessageType;
        }

        public IReadOnlyCollection<Channel> BuildSubscriptions()
            => EnumerateSubscriptions().ToList();

        private IEnumerable<Channel> EnumerateSubscriptions()
        {
            if (_heartbeatChannel != null)
            {
                yield return _heartbeatChannel;
            }

            if (_tickerChannel != null)
            {
                yield return _tickerChannel;
            }
        }
    }
}
