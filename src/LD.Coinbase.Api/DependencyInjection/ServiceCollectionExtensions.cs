﻿using System;
using LD.Coinbase.Api.Net.Http;
using LD.Coinbase.Api.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LD.Coinbase.Api.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMarketDataClient(this IServiceCollection services)
        {
            services
                .AddTransient<IMarketDataClient, MarketDataClient>()
                .AddSingleton<ThrottlingPolicy>()
                .AddTransient<ThrottlingPolicyHandler>();

            services.AddOptions<CoinbaseClientOptions>().Configure(o =>
            {
                o.BaseAddress = new Uri("https://api.pro.coinbase.com", UriKind.Absolute);
            });

            services
                .AddHttpClient(ClientNames.MarketData, (provider, client) =>
                {
                    client.BaseAddress = provider.GetRequiredService<IOptions<CoinbaseClientOptions>>().Value.BaseAddress;
                    client.DefaultRequestHeaders.Add("User-Agent", "CoinbaseProClient");
                })
                .AddHttpMessageHandler<ThrottlingPolicyHandler>();

            return services;
        }
    }
}