using System;
using System.Net.Http;
using LDZ.Coinbase.Api.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using Moq;
using RichardSzalay.MockHttp;

namespace LDZ.Coinbase.Test.Unit
{
    public static class MockHttpMessageHandlerServiceCollectionExtensions
    {
        public static T CreateClient<T>(this IServiceCollection services, MockHttpMessageHandler mockHttp) where T : notnull => services
            .AddCoinbaseProApi()
            .AddMockHttpClient(mockHttp)
            .BuildServiceProvider()
            .GetRequiredService<T>();

        private static IServiceCollection AddMockHttpClient(this IServiceCollection services, MockHttpMessageHandler messageHandler)
        {
            services.AddSingleton(provider => CreateMockHttpClientFactory(provider, messageHandler));

            return services;
        }

        private static IHttpClientFactory CreateMockHttpClientFactory(IServiceProvider provider, MockHttpMessageHandler mockHttp)
        {
            var clientFactoryMock = new Mock<IHttpClientFactory>();

            string? clientName = null;
            clientFactoryMock
                .Setup(factory => factory.CreateClient(It.IsAny<string>()))
                .Callback<string>(c => clientName = c)
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
