using System;

namespace LDZ.Coinbase.Api.Net
{
    /// <summary>
    /// A public sandbox is available for testing API connectivity and web trading.  While the sandbox only hosts a subset of the production order books,
    /// all of the exchange functionality is available. Additionally, in this environment you are allowed to add unlimited fake funds for testing.
    /// </summary>
    internal static class EndpointUriNamesSandbox
    {
        public static readonly Uri RestApiUri = new Uri("https://api-public.sandbox.pro.coinbase.com", UriKind.Absolute);

        public static readonly Uri WebsocketFeedUri = new Uri("wss://ws-feed-public.sandbox.pro.coinbase.com", UriKind.Absolute);
    }
}
