using Microsoft.Extensions.Caching.Distributed;
using System.IO;
using System.Net;
using System.Text.Json;
using Weather_API.Models;

namespace Weather_API.Services
{
    public class WeatherService : IWeatherService
    {

        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IDistributedCache _cache;

        private readonly ILogger _logger;


        public WeatherService(HttpClient httpClient,
                              IConfiguration configuration,
                              IDistributedCache cache,
                              ILogger logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _cache = cache;
            _logger = logger;
        }

        public async Task<WeatherResult> GetWeatherAsync(string city)
        {
            // Check if the weather data is already cached
            var cacheKey = $"weather:{city.ToLowerInvariant()}";

            var cachedWeather = await _cache.GetAsync(cacheKey);

            if (cachedWeather != null)
            {
                return await JsonSerializer.DeserializeAsync<WeatherResult>(new MemoryStream(cachedWeather));
            }


            var apiKey = _configuration["VisualCrossing:ApiKey"];
            var url = $"https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/{city}?key={apiKey}";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Weather provider returned {StatusCode} for city {City}",
                    response.StatusCode,
                    city);

                _logger.LogError(
                    $"Failed to retrieve weather for {city}",
                    city);

                return new WeatherResult
                {
                    Success = false,
                    Error = response.StatusCode switch
                    {
                        HttpStatusCode.BadRequest => "Invalid request.",
                        HttpStatusCode.Unauthorized => "Weather API authentication failed.",
                        HttpStatusCode.NotFound => "City not found.",
                        HttpStatusCode.TooManyRequests => "Weather provider rate limit exceeded.",
                        _ => "Weather provider is unavailable."
                    }

                };
            }


            var data = await response.Content.ReadFromJsonAsync<VisualCrossingResponse>();

            var today = data.Days[0];

           

            var weather = new WeatherResponse
            {
                City = data.ResolvedAddress,
                Temperature = today.Temp,
                Condition = today.Conditions
            };

            // Cache the weather data for 1 hour
            var json = JsonSerializer.Serialize(weather);

            await _cache.SetStringAsync(
                cacheKey,
                json,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                });



            return new WeatherResult
            {
                Success = true,
                Data = weather
            };
        }

    }
}
