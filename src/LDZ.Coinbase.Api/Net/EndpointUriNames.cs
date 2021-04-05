using System;

namespace LDZ.Coinbase.Api.Net
{
    internal static class EndpointUriNames
    {
        public static readonly Uri RestApiUri = new Uri("https://api.pro.coinbase.com", UriKind.Absolute);

        public static readonly Uri WebsocketFeedUri = new Uri("wss://ws-feed.pro.coinbase.com", UriKind.Absolute);
    }
}
