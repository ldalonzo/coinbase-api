using System;
using LDZ.Coinbase.Api.Model.Feed;

namespace LDZ.Coinbase.Api.Hosting
{
    public interface IWebSocketSubscriptionsBuilder
    {
        void SubscribeToHeartbeatChannel(Action<HeartbeatMessage> onReceived, params string[] productIds);
    }
}
