using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LD.Coinbase.Api.Net.Http
{
    internal class ThrottlingPolicyHandler : DelegatingHandler
    {
        public ThrottlingPolicyHandler(ThrottlingPolicy policy)
        {
            _policy = policy;
        }

        private readonly ThrottlingPolicy _policy;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await _policy.Throttle(cancellationToken);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
