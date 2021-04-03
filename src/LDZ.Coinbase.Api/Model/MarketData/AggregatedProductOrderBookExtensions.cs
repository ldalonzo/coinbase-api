using System.Linq;

namespace LDZ.Coinbase.Api.Model.MarketData
{
    public static class AggregatedProductOrderBookExtensions
    {
        public static decimal? GetBestBid(this AggregatedProductOrderBook source)
        {
            if (source.Bids != null && source.Bids.Any())
            {
                return source.Bids.OrderByDescending(order => order.Price).First().Price;
            }

            return null;
        }

        public static decimal? GetWorstBid(this AggregatedProductOrderBook source)
        {
            if (source.Bids != null && source.Bids.Any())
            {
                return source.Bids.OrderBy(order => order.Price).First().Price;
            }

            return null;
        }
    }
}
