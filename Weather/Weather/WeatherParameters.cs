namespace Nomnio.CityWeather.SupportClasses
{
    public class WeatherParameters
    {
        public float Temp { get; set; } //Temperature. In Celsius
        public float Pressure { get; set; } //Atmospheric pressure (on the sea level, if there is no sea_level or grnd_level data), hPa
        public float Humidity { get; set; } //Humidity, %
        public float Temp_min { get; set; } //Minimum temperature at the moment. This is deviation from current temp that is possible for large cities and megalopolises geographically expanded (use these parameter optionally). In Celsius.
        public float Temp_max { get; set; } //Maximum temperature at the moment. This is deviation from current temp that is possible for large cities and megalopolises geographically expanded (use these parameter optionally). In Celsius.
        public float Sea_level { get; set; } //Atmospheric pressure on the sea level, hPa
        public float Grnd_level { get; set; } //Atmospheric pressure on the ground level, hPa
    }
}
