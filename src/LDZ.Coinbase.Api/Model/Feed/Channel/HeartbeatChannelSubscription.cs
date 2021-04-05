using System.Collections.Generic;

namespace LDZ.Coinbase.Api.Model.Feed.Channel
{
    /// <summary>
    /// To receive heartbeat messages for specific products once a second subscribe to the heartbeat channel.
    /// Heartbeats also include sequence numbers and last trade ids that can be used to verify no messages were missed.
    /// </summary>
    public class HeartbeatChannelSubscription : ChannelSubscription
    {
        public ICollection<string>? Products { get; set; }
    }
}
