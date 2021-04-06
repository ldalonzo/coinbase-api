using System.Collections.Generic;
using System.Text.Json;

namespace LDZ.Coinbase.Api.Json.Serialization
{
    internal static class Utf8JsonWriterExtensions
    {
        public const string ProductIds = "product_ids";

        internal static void WriteProducts(this Utf8JsonWriter writer, ICollection<string>? products)
        {
            if (products != null)
            {
                writer.WriteStartArray(ProductIds);

                foreach (var id in products)
                {
                    writer.WriteStringValue(id);
                }

                writer.WriteEndArray();
            }
        }

        internal static List<string> ReadProducts(this ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            if (reader.GetString() != ProductIds)
            {
                throw new JsonException();
            }

            var products = new List<string>();

            reader.Read();
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException();
            }

            reader.Read();
            while (reader.TokenType != JsonTokenType.EndArray)
            {
                if (reader.TokenType != JsonTokenType.String)
                {
                    throw new JsonException();
                }

                products.Add(reader.GetString());
                reader.Read();
            }

            return products;
        }
    }
}
