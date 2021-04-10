using System.Collections.Generic;

namespace LDZ.Coinbase.Api.Model.Feed.Channels
{
    public abstract class ProductsChannel : Channel
    {
        public ICollection<string>? Products { get; set; }
    }
}
