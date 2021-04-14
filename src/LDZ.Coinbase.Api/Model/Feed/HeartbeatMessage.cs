using System;
using System.Text;

namespace LDZ.Coinbase.Api.Model.Feed
{
    /// <summary>
    /// Heartbeats also include <see cref="Sequence">sequence numbers</see> and <see cref="LastTradeId">last trade ids</see> that can be used
    /// to verify no messages were missed.
    /// </summary>
    public class HeartbeatMessage : FeedResponseMessage
    {
        public long? Sequence { get; set; }

        public string? ProductId { get; set; }

        public DateTimeOffset? Time { get; set; }

        public long? LastTradeId { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder("HB");

            if (ProductId != null)
            {
                sb.Append($" {ProductId}");
            }

            if (Sequence != null)
            {
                sb.Append($" #{Sequence}");
            }

            if (Time != null)
            {
                sb.Append($" {Time:HH:mm:ss.fff}");
            }

            return sb.ToString();
        }
    }
}
