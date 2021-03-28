using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using LDZ.Coinbase.Api.Json.Serialization;
using LDZ.Coinbase.Api.Model;
using LDZ.Coinbase.Api.Model.MarketData;

namespace LDZ.Coinbase.Api
{
    internal class MarketDataClient : IMarketDataClient
    {
        public MarketDataClient(IHttpClientFactory factory)
        {
            _factory = factory;

            _options = new JsonSerializerOptions();
            _options.Converters.Add(new DecimalConverter());
            _options.Converters.Add(new TradeSideConverter());
            _options.Converters.Add(new AggregatedOrderJsonConverter());
        }

        private readonly IHttpClientFactory _factory;
        private readonly JsonSerializerOptions _options;

        public async Task<IEnumerable<Product>> GetProductsAsync(CancellationToken cancellationToken = default)
        {
            using var client = _factory.CreateClient(ClientNames.MarketData);

            var response = await client.GetAsync(new Uri("/products", UriKind.Relative), cancellationToken);
            response.EnsureSuccessStatusCode();

            return await JsonSerializer.DeserializeAsync<IEnumerable<Product>>(await response.Content.ReadAsStreamAsync(cancellationToken), _options, cancellationToken);
        }

        public async Task<Product> GetProductAsync(string productId, CancellationToken cancellationToken = default)
        {
            using var client = _factory.CreateClient(ClientNames.MarketData);

            var response = await client.GetAsync(new Uri($"/products/{productId}", UriKind.Relative), cancellationToken);
            response.EnsureSuccessStatusCode();

            return await JsonSerializer.DeserializeAsync<Product>(await response.Content.ReadAsStreamAsync(cancellationToken), _options, cancellationToken);
        }

        public async Task<PaginatedResult<Trade>> GetTradesAsync(string productId, int? after = null, CancellationToken cancellationToken = default)
        {
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

            return PaginatedResultFactory.Create(
                response.Headers,
                await JsonSerializer.DeserializeAsync<IEnumerable<Trade>>(await response.Content.ReadAsStreamAsync(cancellationToken), _options, cancellationToken));
        }

        public async Task<AggregatedProductOrderBook> GetProductOrderBook(string productId, AggregatedProductOrderBookLevel level = AggregatedProductOrderBookLevel.LevelOne, CancellationToken cancellationToken = default)
        {
            using var client = _factory.CreateClient(ClientNames.MarketData);

            var requestUriBuilder = new StringBuilder($"/products/{productId}/book");

            if (level == AggregatedProductOrderBookLevel.LevelTwo)
            {
                requestUriBuilder.Append("?level=2");
            }

            var response = await client.GetAsync(new Uri(requestUriBuilder.ToString(), UriKind.Relative), cancellationToken);
            response.EnsureSuccessStatusCode();

            return await JsonSerializer.DeserializeAsync<AggregatedProductOrderBook>(await response.Content.ReadAsStreamAsync(cancellationToken), _options, cancellationToken);
        }
    }
}
