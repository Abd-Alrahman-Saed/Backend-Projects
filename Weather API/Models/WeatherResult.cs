namespace Weather_API.Models
{
    public class WeatherResult
    {
        public bool Success { get; set; }
        public WeatherResponse? Data { get; set; }
        public string? Error { get; set; }
    }
}
