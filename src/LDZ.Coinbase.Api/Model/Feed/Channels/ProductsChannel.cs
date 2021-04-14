using System.Collections.Generic;

namespace LDZ.Coinbase.Api.Model.Feed.Channels
{
    public abstract class ProductsChannel : FeedChannel
    {
        public static TChannel Create<TChannel>(params string[] products)
            where TChannel : ProductsChannel, new()
        {
            var channel = new TChannel();
            foreach (var product in products)
            {
                channel.Products.Add(product);
            }

            return channel;
        }

        protected ProductsChannel()
        {
            Products = new List<string>();
        }

        public ICollection<string> Products { get; }
    }
}
