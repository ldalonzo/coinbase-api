using System.Text;

namespace LDZ.Coinbase.Api.Model.Feed.Channels
{
    public class TickerChannel : ProductsChannel
    {
        public override string ToString()
        {
            var sb = new StringBuilder("TICKER");

            if (Products != null)
            {
                sb.Append($":{string.Join(",", Products)}");
            }

            return sb.ToString();
        }
    }
}
