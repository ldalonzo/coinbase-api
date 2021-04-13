using System.Text;

namespace LDZ.Coinbase.Api.Model.Feed.Channels
{
    public class HeartbeatChannel : ProductsChannel
    {
        public override string ToString()
        {
            var sb = new StringBuilder("HB");

            if (Products != null)
            {
                sb.Append($":{string.Join(",", Products)}");
            }

            return sb.ToString();
        }
    }
}
