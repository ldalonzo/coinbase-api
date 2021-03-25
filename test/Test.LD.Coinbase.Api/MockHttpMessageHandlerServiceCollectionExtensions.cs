using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using Moq;
using RichardSzalay.MockHttp;

namespace Test.LD.Coinbase.Api
{
    public static class MockHttpMessageHandlerServiceCollectionExtensions
    {
        public static IServiceCollection AddMockHttpClient(this IServiceCollection services, MockHttpMessageHandler messageHandler)
        {
            services.AddSingleton<IHttpClientFactory>(provider => CreateMockHttpClientFactory(provider, messageHandler));

            return services;
        }

        private static IHttpClientFactory CreateMockHttpClientFactory(IServiceProvider provider, MockHttpMessageHandler mockHttp)
        {
            var clientFactoryMock = new Mock<IHttpClientFactory>();

            string clientName = null;
            clientFactoryMock
                .Setup(factory => factory.CreateClient(It.IsAny<string>()))
                .Callback<string>(c => { clientName = c; })
                .Returns(() =>
                {
                    var optionsMonitor = provider.GetRequiredService<IOptionsMonitor<HttpClientFactoryOptions>>();
                    var client = mockHttp.ToHttpClient();

                    var options = optionsMonitor.Get(clientName);
                    foreach (var action in options.HttpClientActions)
                    {
                        action(client);
                    }

                    return client;
                });

            return clientFactoryMock.Object;
        }
    }
}