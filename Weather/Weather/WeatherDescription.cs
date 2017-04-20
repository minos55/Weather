namespace Nomnio.CityWeather.SupportClasses
{
    public class WeatherDescription
    {
        public string Main { get; set; } = string.Empty;//weather parameters (Rain, Snow, Extreme etc.)
        public int Id { get; set; } //Nomnio.Weather condition id
        public string Description { get; set; } = string.Empty;//Nomnio.Weather condition within the main parameter
    }
}
