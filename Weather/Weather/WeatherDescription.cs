namespace Weather.SupportClasses
{
    public class WeatherDescription
    {
        public string main { get; set; } = string.Empty;//weather parameters (Rain, Snow, Extreme etc.)
        public int id { get; set; } //Weather condition id
        public string description { get; set; } = string.Empty;//Weather condition within the main parameter
    }
}
