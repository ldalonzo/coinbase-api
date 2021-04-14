using System.Collections.Generic;
using LDZ.Coinbase.Api.Model.Feed.Channels;

namespace LDZ.Coinbase.Api.Model.Feed
{
    public class SubscribeMessage : FeedRequestMessage
    {
        public ICollection<FeedChannel>? Channels { get; set; }
    }
}
