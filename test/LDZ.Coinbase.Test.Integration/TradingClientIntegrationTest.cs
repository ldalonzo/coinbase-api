using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace LDZ.Coinbase.Test.Integration
{
    [Collection(nameof(CoinbaseRestApiCollection))]
    public class TradingClientIntegrationTest
    {
        private readonly CoinbaseRestApiFixture _fixture;

        public TradingClientIntegrationTest(CoinbaseRestApiFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact(Skip = "Need to create an order first")]
        public async Task GetOrders()
        {
            var orders = await _fixture.TradingClient.ListOrdersAsync();
            orders.ShouldNotBeEmpty();
        }
    }
}
