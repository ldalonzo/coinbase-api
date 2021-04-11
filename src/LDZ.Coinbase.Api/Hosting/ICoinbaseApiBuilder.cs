using System;
using LDZ.Coinbase.Api.Options;
using Microsoft.Extensions.Options;

namespace LDZ.Coinbase.Api.Hosting
{
    public interface ICoinbaseApiBuilder
    {
        ICoinbaseApiBuilder ConfigureApiKey(Action<OptionsBuilder<CoinbaseApiKeyOptions>> configureApiKey);

        ICoinbaseApiBuilder ConfigureFeed(Action<IWebSocketSubscriptionsBuilder> configureFeed);
    }
}
