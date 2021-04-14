using System.Collections.Generic;
using System.Text;
using LDZ.Coinbase.Api.Model.Feed.Channels;

namespace LDZ.Coinbase.Api.Model.Feed
{
    /// <summary>
    /// When subscribing to the <see cref="Level2Channel"/> it will send a message with the type snapshot and the corresponding <see cref="ProductId"/>.
    /// <see cref="Bids"/> and <see cref="Asks"/> are arrays of <see cref="PriceSize"/> tuples and represent the entire order book.
    /// </summary>
    /// <seealso cref="Level2UpdateMessage"/>
    public class Level2SnapshotMessage : Level2Message
    {
        public IReadOnlyList<PriceSize>? Bids { get; set; }

        public IReadOnlyList<PriceSize>? Asks { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder("snapshot");

            if (ProductId != null)
            {
                sb.Append($" {ProductId}");
            }

            return sb.ToString();
        }
    }
}
