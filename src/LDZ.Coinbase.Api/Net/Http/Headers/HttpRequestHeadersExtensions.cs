using System.Net.Http.Headers;

namespace LDZ.Coinbase.Api.Net.Http.Headers
{
    internal static class HttpRequestHeadersExtensions
    {
        public static void AddCoinbaseUserAgent(this HttpRequestHeaders headers)
            => headers.Add("User-Agent", "CoinbaseProClient");
    }
}
