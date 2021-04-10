namespace LDZ.Coinbase.Api.Model.Feed
{
    public class PriceLevelUpdate
    {
        public OrderSide Side { get; set; }

        public decimal Price { get; set; }

        /// <remarks>Please note that size is the updated size at that price level, not a delta. A size of "0" indicates the price level can be removed.</remarks>
        public decimal Size { get; set; }
    }
}
