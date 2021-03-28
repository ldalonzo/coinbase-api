[![CI](https://github.com/ldalonzo/coinbase-api/actions/workflows/ci.yml/badge.svg)](https://github.com/ldalonzo/coinbase-api/actions/workflows/ci.yml) [![nuget](https://img.shields.io/nuget/vpre/LDZ.Coinbase.Api)](https://www.nuget.org/packages/LDZ.Coinbase.Api)

# Coinbase Pro API
A .NET client for the [Coinbase Pro REST API](https://docs.pro.coinbase.com/#api).

## Quick start

```powershell
dotnet add package LDZ.Coinbase.Api
```

```csharp
var client = MarketDataClientFactory.CreateClient();
var trades = await client.GetTradesAsync("BTC-USD");
```
