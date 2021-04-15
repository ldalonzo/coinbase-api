namespace LDZ.Coinbase.Api.Hosting
{
    public interface IWebSocketSubscriptionsBuilder
    {
        /// <summary>
        /// To receive heartbeat messages for specific products once a second subscribe to the heartbeat channel.
        /// Heartbeats also include sequence numbers and last trade ids that can be used to verify no messages were missed.
        /// </summary>
        IWebSocketSubscriptionsBuilder AddHeartbeatChannel(params string[] productIds);

        /// <summary>
        /// The ticker channel provides real-time price updates every time a match happens. It batches updates in case of cascading matches,
        /// greatly reducing bandwidth requirements.
        /// </summary>
        IWebSocketSubscriptionsBuilder AddTickerChannel(params string[] productIds);

        /// <summary>
        /// The easiest way to keep a snapshot of the order book is to use the level2 channel. It guarantees delivery of all updates.
        /// </summary>
        IWebSocketSubscriptionsBuilder AddLevel2Channel(params string[] productIds);
    }
}
