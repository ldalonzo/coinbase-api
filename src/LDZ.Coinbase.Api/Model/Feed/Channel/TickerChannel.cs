using System.Collections.Generic;
using System.Text;

namespace LDZ.Coinbase.Api.Model.Feed.Channel
{
    /// <summary>
    /// 
    /// </summary>
    public class TickerChannel : ChannelSubscription
    {
        public ICollection<string>? Products { get; set; }

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
