namespace LDZ.Coinbase.Api.Model.Feed.Channels
{
    /// <summary>
    /// The easiest way to keep a snapshot of the order book is to use the level2 channel. It guarantees delivery of all updates,
    /// which reduce a lot of the overhead required when consuming the full channel.
    /// </summary>
    public class Level2Channel : ProductsChannel
    {
    }
}
