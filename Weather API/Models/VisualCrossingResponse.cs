namespace Weather_API.Models
{
    public class VisualCrossingResponse
    {
        
        public string ResolvedAddress { get; set; }
        public List<WeatherDay> Days { get; set; }
        
    }
}
