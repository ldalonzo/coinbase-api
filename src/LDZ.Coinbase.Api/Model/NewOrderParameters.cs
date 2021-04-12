using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LDZ.Coinbase.Api.Model
{
    public class NewOrderParameters
    {
        [Required]
        [JsonPropertyName("product_id")]
        public string? ProductId { get; set; }

        [Required]
        [JsonPropertyName("side")]
        public OrderSide? Side { get; set; }

        [JsonPropertyName("size")]
        public decimal? Size { get; set; }

        [JsonPropertyName("price")]
        public decimal? Price { get; set; }
    }
}
