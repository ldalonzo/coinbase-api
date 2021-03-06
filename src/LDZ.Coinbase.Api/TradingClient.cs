using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LDZ.Coinbase.Api.Model;
using LDZ.Coinbase.Api.Net.Http;
using Microsoft.Extensions.Options;

namespace LDZ.Coinbase.Api
{
    internal class TradingClient : ITradingClient
    {
        public TradingClient(IHttpClientFactory factory, IOptions<JsonSerializerOptions> serializerOptions)
        {
            _factory = factory;
            _serializerOptions = serializerOptions.Value;
        }

        private readonly IHttpClientFactory _factory;
        private readonly JsonSerializerOptions _serializerOptions;

        public async Task<Order?> PlaceNewOrderAsync(NewOrderParameters newOrder, CancellationToken cancellationToken = default)
        {
            using var client = _factory.CreateClient(ClientNames.TradingClient);

            var content = JsonContent.Create(newOrder, options: _serializerOptions);
            var response = await client.PostAsync(new Uri("/orders", UriKind.Relative), content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<Order>(json, _serializerOptions);
        }

        public async Task<Guid?> CancelOrder(Guid orderId, string? productId = null, CancellationToken cancellationToken = default)
        {
            using var client = _factory.CreateClient(ClientNames.TradingClient);

            var requestUriBuilder = new StringBuilder($"/orders/{orderId}");
            if (productId != null)
            {
                requestUriBuilder.Append($"?product_id={productId}");
            }

            var response = await client.DeleteAsync(requestUriBuilder.ToString(), cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<Guid>(json, _serializerOptions);
        }

        public async Task<IReadOnlyCollection<Guid>?> CancelAllOrders(string? productId = null, CancellationToken cancellationToken = default)
        {
            using var client = _factory.CreateClient(ClientNames.TradingClient);

            var response = await client.DeleteAsync(new Uri("/orders", UriKind.Relative), cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<IReadOnlyCollection<Guid>>(json, _serializerOptions);
        }

        public async Task<PaginatedResult<Order>?> ListOrdersAsync(CancellationToken cancellationToken = default)
        {
            using var client = _factory.CreateClient(ClientNames.TradingClient);

            var response = await client.GetAsync(new Uri("/orders", UriKind.Relative), cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            return PaginatedResultFactory.Create(response.Headers, JsonSerializer.Deserialize<IReadOnlyCollection<Order>>(json, _serializerOptions));
        }

        public async Task<Order?> GetOrderAsync(Guid id, CancellationToken cancellationToken = default)
        {
            using var client = _factory.CreateClient(ClientNames.TradingClient);

            var response = await client.GetAsync(new Uri($"/orders/{id}", UriKind.Relative), cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<Order>(json, _serializerOptions);
        }
    }
}
