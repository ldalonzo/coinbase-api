using System;

namespace LDZ.Coinbase.Api.Options
{
    public class CoinbaseApiOptions
    {
        public Uri? RestApiBaseUri { get; set; }

        public Uri? WebsocketFeedUri { get; set; }
    }
}
