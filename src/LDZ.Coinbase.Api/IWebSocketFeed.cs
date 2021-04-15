using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using LDZ.Coinbase.Api.Hosting;
using LDZ.Coinbase.Api.Model.Feed;

namespace LDZ.Coinbase.Api
{
    public interface IWebSocketFeed
    {
        ChannelReader<FeedResponseMessage> ChannelReader { get; }

        Task ConnectAsync(CancellationToken cancellationToken = default);

        Task Subscribe(Action<IWebSocketSubscriptionsBuilder> builder, CancellationToken cancellationToken = default);

        Task StopAsync(CancellationToken cancellationToken = default);
    }
}
