using System.Text;

namespace LDZ.Coinbase.Api.Model.Feed.Channels
{
    /// <summary>
    /// The ticker channel provides real-time price updates every time a match happens.
    /// It batches updates in case of cascading matches, greatly reducing bandwidth requirements.
    /// </summary>
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
