using System;
using LDZ.Coinbase.Api.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LDZ.Coinbase.Api.Hosting
{
    internal class CoinbaseApiBuilder : ICoinbaseApiBuilder
    {
        public CoinbaseApiBuilder(IServiceCollection services)
        {
            Services = services;
        }

        private IServiceCollection Services { get; }

        public ICoinbaseApiBuilder ConfigureApiKey(Action<OptionsBuilder<CoinbaseApiKeyOptions>> configureApiKey)
        {
            var builder = Services.AddOptions<CoinbaseApiKeyOptions>();
            configureApiKey(builder);

            return this;
        }
    }
}
