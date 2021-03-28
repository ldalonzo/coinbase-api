using Xunit;

namespace LDZ.Coinbase.Test.Integration
{
    [CollectionDefinition(nameof(MarketDataCollection))]
    public class MarketDataCollection : ICollectionFixture<MarketDataClientFixture>
    {
    }
}
