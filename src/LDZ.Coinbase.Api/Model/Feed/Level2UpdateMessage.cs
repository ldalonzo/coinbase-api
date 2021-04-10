namespace LDZ.Coinbase.Api.Model.Feed
{
    /// <summary>
    /// Subsequent updates will have the type l2update. The changes property of l2updates is an array with [side, price, size] tuples.
    /// The time property of l2update is the time of the event as recorded by our trading engine.
    /// Please note that size is the updated size at that price level, not a delta. A size of "0" indicates the price level can be removed.
    /// </summary>
    /// <seealso cref="Level2SnapshotMessage"/>
    public class Level2UpdateMessage : FeedResponseMessage
    {

    }
}
