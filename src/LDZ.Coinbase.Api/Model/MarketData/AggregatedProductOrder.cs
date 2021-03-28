namespace LDZ.Coinbase.Api.Model.MarketData
{
    /// <summary>
    /// Aggregated levels return only one size for each active price (as if there was only a single order for that size at the level).
    /// </summary>
    public class AggregatedProductOrder
    {
        public decimal Price { get; set; }

        /// <summary>
        /// The sum of the size of the orders at <see cref="Price"/>.
        /// </summary>
        public decimal Size { get; set; }

        /// <summary>
        /// The count of orders at <see cref="Price"/>.
        /// </summary>
        public int NumOrders { get; set; }
    }
}
