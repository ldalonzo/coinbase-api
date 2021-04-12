using System;
using System.Threading.Tasks;
using LDZ.Coinbase.Api;
using LDZ.Coinbase.Api.DependencyInjection;
using LDZ.Coinbase.Api.Hosting;
using LDZ.Coinbase.Api.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LDZ.Coinbase.Test.Integration
{
    public class CoinbaseRestApiFixture : IAsyncLifetime
    {
        public IServiceProvider ServiceProvider => _serviceProvider;

        private readonly ServiceProvider _serviceProvider;

        public CoinbaseRestApiFixture()
        {
            var services = new ServiceCollection();

            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            _serviceProvider = services
                .AddCoinbaseProApi(builder => builder.ConfigureApiKey(apiKey =>
                    {
                        apiKey.Key = configuration["CoinbaseApiKey:Key"];
                        apiKey.Passphrase = configuration["CoinbaseApiKey:Passphrase"];
                        apiKey.Secret = configuration["CoinbaseApiKey:Secret"];
                    }),
                    api => api.UseSandbox())
                .BuildServiceProvider();
        }

        public async Task InitializeAsync()
        {
            await CancelAllOrders();
        }

        private async Task CancelAllOrders()
        {
            using var scope = _serviceProvider.CreateScope();
            var tradingClient = scope.ServiceProvider.GetRequiredService<ITradingClient>();
            await tradingClient.CancelAllOrders();
        }

        public async Task DisposeAsync()
        {
            await CancelAllOrders();
            await _serviceProvider.DisposeAsync();
        }
    }
}
