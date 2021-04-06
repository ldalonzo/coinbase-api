using System.Collections.Generic;

namespace LDZ.Coinbase.Api.Model.Feed.Channel
{
    /// <summary>
    /// 
    /// </summary>
    public class TickerChannel : ChannelSubscription
    {
        public ICollection<string>? Products { get; set; }
    }
}