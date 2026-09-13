namespace Weather_API.Models
{
    public class WeatherDay
    {
        public string Datetime { get; set; }
        public double Temp { get; set; }
        public double Tempmax { get; set; }
        public double Tempmin { get; set; }
        public string Conditions { get; set; }
    }
}
