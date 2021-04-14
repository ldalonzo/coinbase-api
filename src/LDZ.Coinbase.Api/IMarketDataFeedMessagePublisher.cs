using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using LDZ.Coinbase.Api.Model.Feed;

namespace LDZ.Coinbase.Api
{
    public interface IMarketDataFeedMessagePublisher
    {
        /// <summary>
        /// To receive heartbeat messages for specific products once a second subscribe to the heartbeat channel.
        /// Heartbeats also include sequence numbers and last trade ids that can be used to verify no messages were missed.
        /// </summary>
        ChannelReader<HeartbeatMessage> SubscribeToHeartbeatChannel(params string[] productIds);

        /// <summary>
        /// The ticker channel provides real-time price updates every time a match happens. It batches updates in case of cascading matches,
        /// greatly reducing bandwidth requirements.
        /// </summary>
        ChannelReader<TickerMessage> SubscribeToTickerChannel(params string[] productIds);

        /// <summary>
        /// The easiest way to keep a snapshot of the order book is to use the level2 channel. It guarantees delivery of all updates.
        /// </summary>
        ChannelReader<Level2Message> SubscribeToLevel2Channel(params string[] productIds);

        Task StartAsync(CancellationToken cancellationToken = default);

        Task StopAsync(CancellationToken cancellationToken = default);
    }
}
