using System.Text.Json.Serialization;

namespace LDZ.Coinbase.Api.Model.MarketData
{
    public class Product
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }

        [JsonPropertyName("base_currency")]
        public string BaseCurrency { get; set; }

        [JsonPropertyName("quote_currency")]
        public string QuoteCurrency { get; set; }

        /// <summary>
        /// The minimum increment for the <see cref="BaseCurrency"/>.
        /// </summary>
        [JsonPropertyName("base_increment")]
        public decimal BaseIncrement { get; set; }

        /// <summary>
        /// The min order price as well as the price increment.
        /// </summary>
        [JsonPropertyName("quote_increment")]
        public decimal QuoteIncrement { get; set; }

        /// <summary>
        /// Min order size.
        /// </summary>
        [JsonPropertyName("base_min_size")]
        public decimal BaseMinSize { get; set; }

        /// <summary>
        /// Max order size.
        /// </summary>
        [JsonPropertyName("base_max_size")]
        public decimal BaseMaxSize { get; set; }

        public override string ToString()
            => $"#{Id} {DisplayName}";
    }
}
