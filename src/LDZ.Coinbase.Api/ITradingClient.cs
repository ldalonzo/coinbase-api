using System.Threading;
using System.Threading.Tasks;
using LDZ.Coinbase.Api.Model;

namespace LDZ.Coinbase.Api
{
    /// <summary>
    ///  Trading APIs require authentication and provide access to placing orders and other account information.
    /// </summary>
    public interface ITradingClient
    {
        /// <summary>
        /// List your current open orders from the profile that the API key belongs to.  Only open or un-settled orders are returned.
        /// As soon as an order is no longer open and settled, it will no longer appear in the default request.
        /// </summary>
        Task<PaginatedResult<Order>> ListOrdersAsync(CancellationToken cancellationToken = default);
    }
}
