using System.Collections.Generic;
using LDZ.Coinbase.Api.Model.Feed.Channel;

namespace LDZ.Coinbase.Api.Model.Feed
{
    public class SubscribeMessage : FeedMessage
    {
        public ICollection<ChannelSubscription>? Channels { get; set; }
    }
}
