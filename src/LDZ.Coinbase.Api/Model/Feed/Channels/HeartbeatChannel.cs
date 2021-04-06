using System.Collections.Generic;
using System.Text;

namespace LDZ.Coinbase.Api.Model.Feed.Channels
{
    /// <summary>
    /// To receive <see cref="HeartbeatMessage">heartbeat messages</see> for specific <see cref="Products"/> once a second subscribe
    /// to the heartbeat channel.
    /// </summary>
    public class HeartbeatChannel : Channel
    {
        public ICollection<string>? Products { get; set; }

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
