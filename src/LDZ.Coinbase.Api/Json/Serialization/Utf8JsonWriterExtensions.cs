using System.Collections.Generic;
using System.Text.Json;

namespace LDZ.Coinbase.Api.Json.Serialization
{
    internal static class Utf8JsonWriterExtensions
    {
        internal static void WriteProducts(this Utf8JsonWriter writer, ICollection<string>? products)
        {
            if (products != null)
            {
                writer.WriteStartArray("product_ids");

                foreach (var id in products)
                {
                    writer.WriteStringValue(id);
                }

                writer.WriteEndArray();
            }
        }
    }
}
