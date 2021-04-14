using System.Collections.Generic;

namespace LDZ.Coinbase.Api.Model.Feed.Channels
{
    public abstract class ProductsChannel : FeedChannel
    {
        public ICollection<string>? Products { get; set; }
    }
}
