using Weather_API.Models;

namespace Weather_API.Services
{
    public interface IWeatherService
    {
        Task<WeatherResult> GetWeatherAsync(string city);

    }
}
