using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Weather_API.Models;
using Weather_API.Services;

namespace Weather_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IWeatherService _weatherService;

        public WeatherForecastController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }


        [EnableRateLimiting("weather")]
        [HttpGet("{city:minlength(2)}")]
        public async Task<ActionResult<WeatherResponse>> GetWeather(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                return BadRequest("City is required.");
            }

            var result = await _weatherService.GetWeatherAsync(city);

            if (!result.Success)
            {
                return StatusCode(502, result.Error);
            }

            return Ok(result.Data);
        }
    }
}
