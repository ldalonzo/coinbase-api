using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LDZ.Coinbase.Api.Model.MarketData
{
    public class AggregatedProductOrderBook
    {
        [JsonPropertyName("sequence")]
        public long Sequence { get; set; }
        
        [JsonPropertyName("bids")]
        public IEnumerable<AggregatedProductOrder>? Bids { get; set; }

        [JsonPropertyName("asks")]
        public IEnumerable<AggregatedProductOrder>? Asks { get; set; }
    }
}
