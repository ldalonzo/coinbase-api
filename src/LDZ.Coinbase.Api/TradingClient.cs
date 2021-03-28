using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LDZ.Coinbase.Api.Model;

namespace LDZ.Coinbase.Api
{
    internal class TradingClient : ITradingClient
    {
        public TradingClient(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        private readonly IHttpClientFactory _factory;

        public async Task<PaginatedResult<Order>> ListOrdersAsync(CancellationToken cancellationToken = default)
        {
            using var client = _factory.CreateClient(ClientNames.TradingClient);

            var response = await client.GetAsync(new Uri("/orders", UriKind.Relative), cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            return PaginatedResultFactory.Create(response.Headers, JsonSerializer.Deserialize<IEnumerable<Order>>(json));
        }
    }
}
