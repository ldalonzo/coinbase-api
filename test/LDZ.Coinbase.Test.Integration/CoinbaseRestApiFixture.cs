using System.Threading.Tasks;
using LDZ.Coinbase.Api;
using LDZ.Coinbase.Api.Hosting;
using LDZ.Coinbase.Api.Options;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace LDZ.Coinbase.Test.Integration
{
    public class CoinbaseRestApiFixture : IAsyncLifetime
    {
        public CoinbaseApiFactory ApiFactory { get; }

        public CoinbaseRestApiFixture()
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            ApiFactory = CoinbaseApiFactory.Create(builder => builder.ConfigureApiKey(apiKey =>
                {
                    apiKey.Key = configuration["CoinbaseApiKey:Key"];
                    apiKey.Passphrase = configuration["CoinbaseApiKey:Passphrase"];
                    apiKey.Secret = configuration["CoinbaseApiKey:Secret"];
                }),
                api => api.UseSandbox());
        }

        public async Task InitializeAsync()
        {
            await CancelAllOrders();
        }

        private async Task CancelAllOrders()
        {
            var tradingClient = ApiFactory.CreateTradingClient();
            await tradingClient.CancelAllOrders();
        }

        public async Task DisposeAsync()
        {
            await CancelAllOrders();

            ApiFactory.Dispose();
        }
    }
}
