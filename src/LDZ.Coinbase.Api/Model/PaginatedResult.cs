using System.Collections;
using System.Collections.Generic;

namespace LDZ.Coinbase.Api.Model
{
    /// <summary>
    /// Coinbase Pro uses cursor pagination for all REST requests which return arrays. Cursor pagination allows for fetching results before and after the current page
    /// of results and is well suited for realtime data. Endpoints like /trades, /fills, /orders, return the latest items by default.
    /// To retrieve more results subsequent requests should specify which direction to paginate based on the data previously returned.
    /// </summary>
    public class PaginatedResult<T> : IEnumerable<T>
    {
        public PaginatedResult(IReadOnlyCollection<T> value)
        {
            Value = value;
        }

        /// <summary>
        /// The cursor id to use in your next request for the page before the current one.  The page before is a newer page and not
        /// one that happened before in chronological time.
        /// </summary>
        public int? Before { get; set; }

        /// <summary>
        /// The cursor id to use in your next request for the page after this one.  The page after is an older page and not one that
        /// happened after this one in chronological time.
        /// </summary>
        public int? After { get; set; }

        public IReadOnlyCollection<T> Value { get; }

        public IEnumerator<T> GetEnumerator()
            => Value.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
