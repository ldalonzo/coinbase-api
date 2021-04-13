using System;
using LDZ.Coinbase.Api.Model.Feed;

namespace LDZ.Coinbase.Api.Hosting
{
    public interface IWebSocketSubscriptionsBuilder
    {
        /// <summary>
        /// To receive heartbeat messages for specific products once a second subscribe to the heartbeat channel.
        /// Heartbeats also include sequence numbers and last trade ids that can be used to verify no messages were missed.
        /// </summary>
        void SubscribeToHeartbeatChannel(Action<HeartbeatMessage> onReceived, params string[] productIds);

        /// <summary>
        /// The ticker channel provides real-time price updates every time a match happens. It batches updates in case of cascading matches,
        /// greatly reducing bandwidth requirements.
        /// </summary>
        void SubscribeToTickerChannel(Action<TickerMessage> onReceived, params string[] productIds);
    }
}
