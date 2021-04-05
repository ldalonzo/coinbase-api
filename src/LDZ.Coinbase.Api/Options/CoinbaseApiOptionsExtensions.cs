using LDZ.Coinbase.Api.Net;
using Microsoft.Extensions.Options;

namespace LDZ.Coinbase.Api.Options
{
    public static class CoinbaseApiOptionsExtensions
    {
        public static OptionsBuilder<CoinbaseApiOptions> UseProduction(this OptionsBuilder<CoinbaseApiOptions> configureOptions)
            => configureOptions.Configure(o => o.RestApiBaseUri = EndpointUriNames.RestApiUri);

        public static OptionsBuilder<CoinbaseApiOptions> UseSandbox(this OptionsBuilder<CoinbaseApiOptions> configureOptions)
            => configureOptions.Configure(o => o.RestApiBaseUri = EndpointUriNamesSandbox.RestApiUri);
    }
}
