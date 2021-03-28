namespace LDZ.Coinbase.Api.Model.MarketData
{
    /// <summary>
    /// The trade side indicates the maker order side.
    /// The maker order is the order that was open on the order book. buy side indicates a down-tick because the maker was a buy order and their order was removed.
    /// Conversely, sell side indicates an up-tick.
    /// </summary>
    public enum TradeSide
    {
        Buy,
        Sell
    }
}
