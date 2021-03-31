using Xunit;

namespace LDZ.Coinbase.Test.Integration
{
    [CollectionDefinition(nameof(CoinbaseRestApiCollection))]
    public class CoinbaseRestApiCollection : ICollectionFixture<CoinbaseRestApiFixture>
    {
    }
}
