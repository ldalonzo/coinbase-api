using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace LD.Coinbase.Api.Net.Http
{
    internal class ThrottlingPolicy
    {
        public ThrottlingPolicy(ILogger<ThrottlingPolicy> log)
        {
            _log = log;
        }

        private const int MaxCallsPerSecond = 3;

        private readonly ILogger<ThrottlingPolicy> _log;

        private DateTime? lastRequestTime;
        private readonly SemaphoreSlim semaphore = new(1);

        public async Task Throttle(CancellationToken cancellationToken)
        {
            await semaphore.WaitAsync(cancellationToken);

            try
            {
                if (lastRequestTime != null)
                {
                    var earliestTime = lastRequestTime.Value.Add(TimeSpan.FromMilliseconds(1_000 / MaxCallsPerSecond));
                    var delta = earliestTime - DateTime.UtcNow;
                    if (delta.Ticks > 0)
                    {
                        _log.LogDebug($"Throttling request. Waiting {delta.TotalMilliseconds}ms.");
                        await Task.Delay(delta, cancellationToken);
                    }
                }

                lastRequestTime = DateTime.UtcNow;
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
