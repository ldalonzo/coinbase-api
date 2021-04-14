using System;
using System.Collections.Generic;
using System.Text;

namespace LDZ.Coinbase.Api.Model.Feed
{
    /// <seealso cref="Level2SnapshotMessage"/>
    public class Level2UpdateMessage : Level2Message
    {
        /// <summary>
        /// The time of the event as recorded by the trading engine.
        /// </summary>
        public DateTimeOffset? Time { get; set; }

        public IReadOnlyList<PriceLevelUpdate>? Changes { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder("l2update");

            if (Time != null)
            {
                sb.Append($" {Time:HH:mm:ss.fff}");
            }

            if (ProductId != null)
            {
                sb.Append($" {ProductId}");
            }

            return sb.ToString();
        }
    }
}
