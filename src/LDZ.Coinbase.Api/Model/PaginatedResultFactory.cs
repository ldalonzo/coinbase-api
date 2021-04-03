using System.Collections.Generic;
using System.Net.Http.Headers;
using LDZ.Coinbase.Api.Net.Http.Headers;

namespace LDZ.Coinbase.Api.Model
{
    internal static class PaginatedResultFactory
    {
        public static PaginatedResult<T> Create<T>(HttpHeaders headers, IEnumerable<T> value)
        {
            var result = new PaginatedResult<T>
            {
                Value = value
            };

            if (headers.TryParseInt(HeaderNames.After, out var after))
            {
                result.After = after;
            }

            if (headers.TryParseInt(HeaderNames.Before, out var before))
            {
                result.Before = before;
            }

            return result;
        }
    }
}
