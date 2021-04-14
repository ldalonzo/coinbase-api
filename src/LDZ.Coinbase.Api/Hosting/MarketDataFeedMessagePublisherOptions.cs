using System;
using System.Collections.Generic;
using LDZ.Coinbase.Api.Model.Feed;
using LDZ.Coinbase.Api.Model.Feed.Channels;

namespace LDZ.Coinbase.Api.Hosting
{
    internal class MarketDataFeedMessagePublisherOptions
    {
        public IReadOnlyCollection<FeedChannel>? Subscriptions { get; set; }

        public IReadOnlyDictionary<Type, Action<FeedResponseMessage>>? Handlers { get; set; }
    }
}
