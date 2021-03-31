using System;
using System.Text.Json.Serialization;
using LDZ.Coinbase.Api.Model.MarketData;

namespace LDZ.Coinbase.Api.Model
{
    public class Order
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("size")]
        public decimal Size { get; set; }

        [JsonPropertyName("product_id")]
        public string ProductId { get; set; }

        [JsonPropertyName("side")]
        public OrderSide Side { get; set; }

        [JsonPropertyName("funds")]
        public decimal Funds { get; set; }

        [JsonPropertyName("specified_funds")]
        public decimal SpecifiedFunds { get; set; }

        [JsonPropertyName("type")]
        public OrderType Type { get; set; }

        [JsonPropertyName("post_only")]
        public bool PostOnly { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("done_at")]
        public DateTime DoneAt { get; set; }

        [JsonPropertyName("fill_fees")]
        public decimal FillFees { get; set; }

        [JsonPropertyName("filled_size")]
        public decimal FilledSize { get; set; }

        [JsonPropertyName("executed_value")]
        public decimal ExecutedValue { get; set; }

        [JsonPropertyName("settled")]
        public bool Settled { get; set; }
    }
}
