using System;
using LDZ.Coinbase.Api.Options;

namespace LDZ.Coinbase.Api.Hosting
{
    public static class CoinbaseApiBuilderExtensions
    {
        public static ICoinbaseApiBuilder ConfigureApiKey(this ICoinbaseApiBuilder builder, Action<CoinbaseApiKeyOptions> configureApiKey)
            => builder.ConfigureApiKey(b => b.Configure(configureApiKey));
    }
}
