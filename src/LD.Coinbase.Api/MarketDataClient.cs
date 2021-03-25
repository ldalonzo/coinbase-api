using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LD.Coinbase.Api.Json.Serialization;
using LD.Coinbase.Api.Model.MarketData;

namespace LD.Coinbase.Api
{
    internal class MarketDataClient : IMarketDataClient
    {
        public MarketDataClient(IHttpClientFactory factory)
        {
            _factory = factory;

            _options = new JsonSerializerOptions();
            _options.Converters.Add(new DecimalConverter());
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
    }
}
