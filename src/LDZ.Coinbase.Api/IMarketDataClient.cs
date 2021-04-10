using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LDZ.Coinbase.Api.Model;
using LDZ.Coinbase.Api.Model.MarketData;

namespace LDZ.Coinbase.Api
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
        Task<IReadOnlyCollection<Product>?> GetProductsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get market data for a specific currency pair.
        /// </summary>
        Task<Product?> GetProductAsync(string productId, CancellationToken cancellationToken = default);

        /// <summary>
        /// List the latest <see cref="Trade"/>s for a <see cref="Product"/>.
        /// </summary>
        Task<PaginatedResult<Trade>?> GetTradesAsync(string productId, int? after = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a list of open orders for a product. The amount of detail shown can be customized with the <paramref name="level"/> parameter.
        /// </summary>
        Task<AggregatedProductOrderBook?> GetProductOrderBookAsync(string productId, AggregatedProductOrderBookLevel level = AggregatedProductOrderBookLevel.LevelOne, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get the API server time.
        /// </summary>
        Task<ApiServerTime?> GetTimeAsync(CancellationToken cancellationToken = default);
    }
}
