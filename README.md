[![CI](https://github.com/ldalonzo/coinbase-api/actions/workflows/ci.yml/badge.svg)](https://github.com/ldalonzo/coinbase-api/actions/workflows/ci.yml) [![nuget](https://img.shields.io/nuget/v/LDZ.Coinbase.Api)](https://www.nuget.org/packages/LDZ.Coinbase.Api) [![codecov](https://codecov.io/gh/ldalonzo/coinbase-api/branch/master/graph/badge.svg?token=8CPL3UFFJZ)](https://codecov.io/gh/ldalonzo/coinbase-api)

# Coinbase Pro API
A .NET client for the [Coinbase Pro REST API](https://docs.pro.coinbase.com/#api).

## Quick start

```powershell
dotnet add package LDZ.Coinbase.Api
```

### Feed APIs
Feed APIs provide market data and are public. By accessing the Coinbase Pro Market Data API, you agree to be bound by the [Coinbase Market Data Terms of Use](https://www.coinbase.com/legal/market_data).
```csharp
var factory = CoinbaseApiFactory.Create();
var marketData = factory.CreateMarketDataClient();

var trades = await marketData.GetTradesAsync("BTC-USD");
```

### Trading APIs
Trading APIs require authentication and provide access to placing orders and other account information.  You must create an API key via the Coinbase Pro website.
```csharp
var factory = CoinbaseApiFactory.Create(builder => builder.ConfigureApiKey(apiKey =>
{
    apiKey.Key = "API_KEY";
    apiKey.Secret = "API_SECRET";
    apiKey.Passphrase = "API_PASS";
}));

var tradingClient = factory.CreateTradingClient();
var orders = await tradingClient.ListOrdersAsync();
```

### Websocket Feed
```csharp
static async Task Main(string[] args)
{
    var factory = CoinbaseApiFactory.Create(builder => builder.ConfigureFeed(feedBuilder =>
    {
        feedBuilder.SubscribeToHeartbeatChannel(OnMessageReceived, "BTC-USD");
    }));

    var dataFeed = await factory.StartMarketDataFeed();

    Console.ReadKey();
    await dataFeed.StopAsync();
}

private static void OnMessageReceived(HeartbeatMessage message)
{
    Console.WriteLine(message);
}
```

## Contributing

### Run integration tests
Integration tests target the public [Coinbase sandbox](https://docs.pro.coinbase.com/#sandbox). You'll need to create an API key and set the following environment variables:
```powershell
 $env:CoinbaseApiKey__Key = "API_KEY"
 $env:CoinbaseApiKey__Secret = "API_SECRET"
 $env:CoinbaseApiKey__Passphrase = "API_PASS"
```

```powershell
cd test\LDZ.Coinbase.Test.Integration
dotnet test --logger:"console;verbosity=detailed"
```
