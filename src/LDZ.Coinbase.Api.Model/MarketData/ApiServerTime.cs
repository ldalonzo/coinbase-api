using System;
using System.Text.Json.Serialization;

namespace LDZ.Coinbase.Api.Model.MarketData
{
    public class ApiServerTime
    {
        [JsonPropertyName("iso")]
        public DateTime Iso { get; set; }

        [JsonPropertyName("epoch")]
        public decimal Epoch { get; set; }
    }
}
