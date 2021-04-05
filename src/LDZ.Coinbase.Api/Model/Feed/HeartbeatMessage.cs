using System;
using System.Text;

namespace LDZ.Coinbase.Api.Model.Feed
{
    public class HeartbeatMessage : FeedResponseMessage
    {
        public string? ProductId { get; set; }

        public DateTime? Time { get; set; }

        public long? LastTradeId { get; set; }

        public long? Sequence { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append("HB");
            if (ProductId != null)
            {
                sb.Append($" {ProductId}");
            }

            if (Sequence != null)
            {
                sb.Append($" #{Sequence}");
            }

            return sb.ToString();
        }
    }
}
