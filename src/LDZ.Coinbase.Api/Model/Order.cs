using System;
using System.Text.Json.Serialization;

namespace LDZ.Coinbase.Api.Model
{
    public class Order
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
    }
}
