using System.Collections.Generic;

namespace LDZ.Coinbase.Api.Model.MarketData
{
    public class TradeTradeIdEqualityComparer : EqualityComparer<Trade>
    {
        public override bool Equals(Trade? x, Trade? y)
            => Equals(x?.TradeId, y?.TradeId);

        public override int GetHashCode(Trade obj)
            => obj.TradeId.GetHashCode();
    }
}
