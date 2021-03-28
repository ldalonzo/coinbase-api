namespace LDZ.Coinbase.Api.Model.MarketData
{
    public enum AggregatedProductOrderBookLevel
    {
        /// <summary>
        /// Only the best bid and ask.
        /// </summary>
        LevelOne = 1,

        /// <summary>
        /// Top 50 bids and asks.
        /// </summary>
        LevelTwo = 2
    }
}
