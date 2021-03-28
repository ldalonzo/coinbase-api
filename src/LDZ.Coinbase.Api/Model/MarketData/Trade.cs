using System;
using System.Text.Json.Serialization;

namespace LDZ.Coinbase.Api.Model.MarketData
{
    public class Trade
    {
        [JsonPropertyName("time")]
        public DateTime Time { get; set; }

        [JsonPropertyName("trade_id")]
        public int TradeId { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("size")]
        public decimal Size { get; set; }

        [JsonPropertyName("side")]
        public TradeSide Side { get; set; }

        public override string ToString()
            => $"#{TradeId} {Time:s} {Side} {Size:N2}@{Price:N3}";
    }
}
