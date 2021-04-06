using System.Collections.Generic;
using System.Linq;
using System.Text;
using LDZ.Coinbase.Api.Model.Feed.Channel;

namespace LDZ.Coinbase.Api.Model.Feed
{
    public class SubscriptionsMessage : FeedResponseMessage
    {
        public ICollection<ChannelSubscription>? Channels { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder("SUBS");

            sb.Append(" [ ");

            if (Channels != null)
            {
                foreach (var channel in Channels)
                {
                    sb.Append(channel);
                }
            }

            sb.Append(" ]");

            return sb.ToString();
        }
    }
}
