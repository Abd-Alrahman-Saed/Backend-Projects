# Weather API

A minimal ASP.NET Core Web API that returns the current weather for a given city. It pulls live data from the [Visual Crossing](https://www.visualcrossing.com/) timeline API, caches responses in Redis, and protects the endpoint with rate limiting.

Part of the [roadmap.sh weather API project](https://roadmap.sh/projects/weather-api-wrapper-service).

## Features

- **Live weather** – current temperature and conditions for any city
- **Redis caching** – successful responses are cached for 1 hour to reduce upstream calls
- **Rate limiting** – fixed-window limiter (10 requests/minute per client) returning `429` when exceeded
- **Swagger UI** – interactive docs available in development
- **Graceful error mapping** – upstream failures are translated to clear HTTP responses

## Tech stack

- [.NET 10](https://dotnet.microsoft.com/) (ASP.NET Core Web API)
- [Visual Crossing Weather API](https://www.visualcrossing.com/)
- [StackExchange.Redis / Distributed Cache](https://www.nuget.org/packages/Microsoft.Extensions.Caching.StackExchangeRedis)
- [Swashbuckle (Swagger)](https://www.nuget.org/packages/Swashbuckle.AspNetCore)
- Built-in ASP.NET Core [rate limiting](https://learn.microsoft.com/aspnet/core/performance/rate-limit)

## Prerequisites

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- A running **Redis** server on `localhost:6379` (e.g., [Windows](https://github.com/redis-windows/redis-windows) / [Docker](https://hub.docker.com/_/redis))
- A [Visual Crossing](https://www.visualcrossing.com/) API key

## Setup

1. Restore and build:

   ```sh
   dotnet restore
   dotnet build
   ```

2. Configure your Visual Crossing API key (kept out of source control via user secrets):

   ```sh
   dotnet user-secrets set "VisualCrossing:ApiKey" "your-api-key"
   ```

3. Make sure Redis is up on `localhost:6379`.

## Running

```sh
dotnet run
```

The API is then available at `http://localhost:5211` with Swagger UI at `/swagger`.

## Usage

### Get weather for a city

```
GET /weatherforecast/{city}
```

`city` must be at least 2 characters long (e.g. `Cairo`, `London`, `90.37727,23.79`).

Successful response (`200 OK`):

```json
{
  "city": "Cairo, Egypt",
  "temperature": 31.5,
  "condition": "Clear"
}
```

Possible errors:

| Status | Meaning |
|--------|---------|
| `400` | City is missing/empty |
| `502` | Upstream weather provider failed (translated error message) |
| `429` | Rate limit exceeded (10 req/min) |

Try it with the provided `Weather API.http` file or any REST client.

## Project structure

```
Weather API/
├── Controllers/
│   └── WeatherForecastController.cs   # GET /weatherforecast/{city}
├── Models/
│   ├── VisualCrossingResponse.cs      # Upstream API payload
│   ├── WeatherDay.cs                  # Single forecast day
│   ├── WeatherResponse.cs             # API response shape
│   └── WeatherResult.cs               # Success/error wrapper
├── Services/
│   ├── IWeatherService.cs             # Service contract
│   └── WeatherService.cs              # Fetch, cache, and map logic
├── Program.cs                         # DI, Redis, rate limiting, Swagger
└── appsettings.json
```
