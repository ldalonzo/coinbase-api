namespace LDZ.Coinbase.Api.Model
{
    /// <summary>
    /// Defines how your order will be executed by the matching engine.
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// Limit orders are both the default and basic order type.  A limit order requires specifying a price and size.
        /// The size is the number of base currency to buy or sell, and the price is the price per base currency.
        /// The limit order will be filled at the price specified or better.
        /// A sell order can be filled at the specified price per base currency or a higher price per base currency and a buy order
        /// can be filled at the specified price or a lower price depending on market conditions. If market conditions cannot fill
        /// the limit order immediately, then the limit order will become part of the open order book until filled by another
        /// incoming order or canceled by the user.
        /// </summary>
        Limit,

        /// <summary>
        /// Market orders differ from limit orders in that they provide no pricing guarantees.
        /// They however do provide a way to buy or sell specific amounts of base currency or fiat without having to specify the price.
        /// Market orders execute immediately and no part of the market order will go on the open order book. Market orders are always
        /// considered takers and incur taker fees. When placing a market order you can specify funds and/or size. Funds will limit
        /// how much of your quote currency account balance is used and size will limit the amount of base currency transacted.
        /// </summary>
        Market
    }
}
