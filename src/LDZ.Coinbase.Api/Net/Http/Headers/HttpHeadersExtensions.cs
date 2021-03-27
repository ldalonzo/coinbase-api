using System.Linq;
using System.Net.Http.Headers;

namespace LDZ.Coinbase.Api.Net.Http.Headers
{
    public static class HttpHeadersExtensions
    {
        public static bool TryParseInt(this HttpHeaders headers, string headerName, out int value)
        {
            if (headers.TryGetValues(headerName, out var headerValues))
            {
                var headerValue = headerValues.FirstOrDefault();
                if (headerValue != null && int.TryParse(headerValue, out value))
                {
                    return true;
                }
            }

            value = default;
            return false;
        }
    }
}
