using System;
using System.Text;
using System.Text.Json.Serialization;

namespace LDZ.Coinbase.Api.Model
{
    public class Order
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("price")]
        public decimal? Price { get; set; }

        [JsonPropertyName("size")]
        public decimal Size { get; set; }

        [JsonPropertyName("product_id")]
        public string? ProductId { get; set; }

        [JsonPropertyName("side")]
        public OrderSide Side { get; set; }

        [JsonPropertyName("funds")]
        public decimal? Funds { get; set; }

        [JsonPropertyName("specified_funds")]
        public decimal? SpecifiedFunds { get; set; }

        [JsonPropertyName("type")]
        public OrderType Type { get; set; }

        [JsonPropertyName("post_only")]
        public bool PostOnly { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("done_at")]
        public DateTime? DoneAt { get; set; }

        [JsonPropertyName("fill_fees")]
        public decimal? FillFees { get; set; }

        [JsonPropertyName("filled_size")]
        public decimal? FilledSize { get; set; }

        [JsonPropertyName("executed_value")]
        public decimal? ExecutedValue { get; set; }

        [JsonPropertyName("settled")]
        public bool Settled { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append($"#{Id} [");

            if (ProductId != null)
            {
                sb.Append($" {ProductId}");
            }

            if (Price != null)
            {
                sb.Append($" {Size}@{Price}");
            }

            sb.Append(" ]");

            return sb.ToString();
        }
    }
}
