using System.Collections.Generic;
using System.Linq;
using LDZ.Coinbase.Api.Model.Feed.Channels;

namespace LDZ.Coinbase.Api.Model.Feed
{
    public class SubscribeMessage : FeedRequestMessage
    {
        public static SubscribeMessage Create(params FeedChannel[] channels)
        {
            return new SubscribeMessage(channels);
        }

        public static SubscribeMessage Create(IEnumerable<FeedChannel> channels)
        {
            return new SubscribeMessage(channels);
        }

        private SubscribeMessage(IEnumerable<FeedChannel> channels)
        {
            Channels = channels.ToList();
        }

        public ICollection<FeedChannel> Channels { get; }
    }
}
