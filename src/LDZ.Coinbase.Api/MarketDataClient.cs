using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using LDZ.Coinbase.Api.Model;
using LDZ.Coinbase.Api.Model.MarketData;
using LDZ.Coinbase.Api.Net;
using Microsoft.Extensions.Options;

namespace LDZ.Coinbase.Api
{
    internal class MarketDataClient : IMarketDataClient
    {
        public MarketDataClient(IHttpClientFactory factory, IOptions<JsonSerializerOptions> serializerOptions)
        {
            _factory = factory;

            _options = serializerOptions.Value;
        }

        private readonly IHttpClientFactory _factory;
        private readonly JsonSerializerOptions _options;

        public async Task<IReadOnlyCollection<Product>?> GetProductsAsync(CancellationToken cancellationToken = default)
        {
            using var client = _factory.CreateClient(ClientNames.MarketData);

            var response = await client.GetAsync(new Uri("/products", UriKind.Relative), cancellationToken);
            response.EnsureSuccessStatusCode();

            return await JsonSerializer.DeserializeAsync<IReadOnlyCollection<Product>>(await response.Content.ReadAsStreamAsync(cancellationToken), _options, cancellationToken);
        }

        public async Task<Product?> GetProductAsync(string productId, CancellationToken cancellationToken = default)
        {
            if (productId == null)
            {
                throw new ArgumentNullException(nameof(productId));
            }

            using var client = _factory.CreateClient(ClientNames.MarketData);

            var response = await client.GetAsync(new Uri($"/products/{productId}", UriKind.Relative), cancellationToken);
            response.EnsureSuccessStatusCode();

            return await JsonSerializer.DeserializeAsync<Product>(await response.Content.ReadAsStreamAsync(cancellationToken), _options, cancellationToken);
        }

        public async Task<PaginatedResult<Trade>?> GetTradesAsync(string productId, int? after = null, CancellationToken cancellationToken = default)
        {
            if (productId == null)
            {
                throw new ArgumentNullException(nameof(productId));
            }

            using var client = _factory.CreateClient(ClientNames.MarketData);

            var requestUriBuilder = new StringBuilder($"/products/{productId}/trades");

            var queryString = HttpUtility.ParseQueryString(string.Empty);
            if (after.HasValue)
            {
                queryString.Add("after", $"{after.Value}");
            }

            if (queryString.HasKeys())
            {
                requestUriBuilder.Append($"?{queryString}");
            }

            var response = await client.GetAsync(requestUriBuilder.ToString(), cancellationToken);
            response.EnsureSuccessStatusCode();

            var trades = await JsonSerializer.DeserializeAsync<IReadOnlyCollection<Trade>>(await response.Content.ReadAsStreamAsync(cancellationToken), _options, cancellationToken);
            return PaginatedResultFactory.Create(response.Headers, trades);
        }

        public async Task<AggregatedProductOrderBook?> GetProductOrderBookAsync(string productId, AggregatedProductOrderBookLevel level = AggregatedProductOrderBookLevel.LevelOne, CancellationToken cancellationToken = default)
        {
            if (productId == null)
            {
                throw new ArgumentNullException(nameof(productId));
            }

            using var client = _factory.CreateClient(ClientNames.MarketData);

            var requestUriBuilder = new StringBuilder($"/products/{productId}/book");

            if (level == AggregatedProductOrderBookLevel.LevelTwo)
            {
                requestUriBuilder.Append("?level=2");
            }

            var response = await client.GetAsync(new Uri(requestUriBuilder.ToString(), UriKind.Relative), cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<AggregatedProductOrderBook>(json, _options);
        }

        public async Task<ApiServerTime?> GetTimeAsync(CancellationToken cancellationToken = default)
        {
            using var client = _factory.CreateClient(ClientNames.MarketData);

            var response = await client.GetAsync(new Uri("/time", UriKind.Relative), cancellationToken);
            response.EnsureSuccessStatusCode();

            return await JsonSerializer.DeserializeAsync<ApiServerTime>(await response.Content.ReadAsStreamAsync(cancellationToken), cancellationToken: cancellationToken);
        }
    }
}
