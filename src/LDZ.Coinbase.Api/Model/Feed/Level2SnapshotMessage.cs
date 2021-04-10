using System.Collections.Generic;
using LDZ.Coinbase.Api.Model.Feed.Channels;

namespace LDZ.Coinbase.Api.Model.Feed
{
    /// <summary>
    /// When subscribing to the <see cref="Level2Channel"/> it will send a message with the type snapshot and the corresponding product_id.
    /// <see cref="Bids"/> and <see cref="Asks"/> are arrays of [price, size] tuples and represent the entire order book.
    /// </summary>
    /// <seealso cref="Level2UpdateMessage"/>
    public class Level2SnapshotMessage : FeedResponseMessage
    {
        public string? ProductId { get; set; }

        public IReadOnlyList<PriceSize>? Asks { get; set; }

        public IReadOnlyList<PriceSize>? Bids { get; set; }
    }
}
