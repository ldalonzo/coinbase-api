using System.Threading;
using System.Threading.Tasks;

namespace LDZ.Coinbase.Api
{
    public interface IMarketDataFeedMessagePublisher
    {
        Task StartAsync(CancellationToken cancellationToken = default);

        Task StopAsync(CancellationToken cancellationToken = default);
    }
}
