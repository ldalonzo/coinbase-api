using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using LDZ.Coinbase.Api.Model.MarketData;

namespace LDZ.Coinbase.Api
{
    public static class MarketDataClientExtensions
    {
        public static async IAsyncEnumerable<Trade> GetAllTradesAsync(this IMarketDataClient client, string productId, int? after = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var trades = await client.GetTradesAsync(productId, after, cancellationToken);

                foreach (var trade in trades)
                {
                    yield return trade;
                }

                after = trades.After;
            }
        }
    }
}
