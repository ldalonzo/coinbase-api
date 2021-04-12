using System.Collections.Generic;
using System.Text;
using LDZ.Coinbase.Api.Model.Feed.Channels;

namespace LDZ.Coinbase.Api.Model.Feed
{
    public class SubscriptionsMessage : FeedResponseMessage
    {
        public ICollection<Channel>? Channels { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder("SUBS");

            sb.Append(" [ ");

            if (Channels != null)
            {
                sb.AppendJoin(", ", Channels);
            }

            sb.Append(" ]");

            return sb.ToString();
        }
    }
}
