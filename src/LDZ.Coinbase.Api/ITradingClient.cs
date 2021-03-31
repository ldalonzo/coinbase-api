﻿using System;
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
        /// You can place two types of orders: <see cref="OrderType.Limit">limit</see> and <see cref="OrderType.Market">market</see>.
        /// Orders can only be placed if your account has sufficient funds. Each profile can have a maximum of 500 open orders on a product.
        /// Once reached, the profile will not be able to place any new orders until the total number of open orders is below 500.
        /// </summary>
        Task<Order> PlaceNewOrderAsync(NewOrderParameters newOrder, CancellationToken cancellationToken = default);

        /// <summary>
        /// List your current open orders from the profile that the API key belongs to.  Only open or un-settled orders are returned.
        /// As soon as an order is no longer open and settled, it will no longer appear in the default request.
        /// </summary>
        Task<PaginatedResult<Order>> ListOrdersAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a single order by order <see cref="id"/>from the profile that the API key belongs to.
        /// </summary>
        /// <remarks>If the order is canceled the response may have status code 404 if the order had no matches.</remarks>
        Task<Order> GetOrderAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
