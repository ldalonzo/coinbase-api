using System;
using System.Text;

namespace LDZ.Coinbase.Api.Model.Feed
{
    public class TickerMessage : FeedResponseMessage
    {
        public long? Sequence { get; set; }

        public string? ProductId { get; set; }

        public DateTimeOffset? Time { get; set; }

        public long? TradeId { get; set; }

        public decimal? Price { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder("TCK");

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

            if (Price != null)
            {
                sb.Append($" @{Price:N2}");
            }

            return sb.ToString();
        }
    }
}
