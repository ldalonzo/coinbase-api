using System;
using System.Threading;
using System.Threading.Tasks;
using LDZ.Coinbase.Api.Model;
using LDZ.Coinbase.Api.Model.MarketData;

namespace LDZ.Coinbase.Api
{
    public static class MarketDataClientExtensions
    {
        public static Task<PaginatedResult<Trade>?> GetTradesAsync(this IMarketDataClient client, Product product, int? after = null, CancellationToken cancellationToken = default)
        {
            if (product.Id == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            return client.GetTradesAsync(product.Id, after, cancellationToken);
        }
    }
}
