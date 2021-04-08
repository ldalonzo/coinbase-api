using System.Collections.Generic;

namespace LDZ.Coinbase.Api.Model.Feed.Channels
{
    public class Level2Channel : Channel
    {
        public ICollection<string>? Products { get; set; }
    }
}
