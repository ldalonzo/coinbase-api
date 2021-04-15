using System.Collections.Generic;
using LDZ.Coinbase.Api.Model.Feed.Channels;

namespace LDZ.Coinbase.Api.Hosting
{
    internal class WebSocketSubscriptionsBuilder
    {
        private HeartbeatChannel? _heartbeatChannel;
        private TickerChannel? _tickerChannel;
        private Level2Channel? _level2Channel;

        public void SubscribeToHeartbeatChannel(params string[] productIds)
        {
            _heartbeatChannel ??= new HeartbeatChannel();

            foreach (var productId in productIds)
            {
                _heartbeatChannel.Products.Add(productId);
            }
        }

        public void SubscribeToTickerChannel(params string[] productIds)
        {
            _tickerChannel ??= new TickerChannel();

            foreach (var productId in productIds)
            {
                _tickerChannel.Products.Add(productId);
            }
        }

        public void SubscribeToLevel2Channel(params string[] productIds)
        {
            _level2Channel ??= new Level2Channel();

            foreach (var productId in productIds)
            {
                _level2Channel.Products.Add(productId);
            }
        }

        public IEnumerable<FeedChannel> BuildSubscriptions()
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
