using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LD.Coinbase.Api.Model;
using LD.Coinbase.Api.Model.MarketData;

namespace LD.Coinbase.Api
{
    /// <summary>
    /// The Market Data API is an unauthenticated set of endpoints for retrieving market data. These endpoints provide snapshots of market data.
    /// </summary>
    /// <remarks>By accessing the Coinbase Pro Market Data API, you agree to be bound by the <see href="https://www.coinbase.com/legal/market_data">Market Data Terms of Use.</see></remarks>
    public interface IMarketDataClient
    {
        /// <summary>
        /// Get a list of available currency pairs for trading.
        /// </summary>
        Task<IEnumerable<Product>> GetProductsAsync(CancellationToken cancellationToken = default);
    }
}
