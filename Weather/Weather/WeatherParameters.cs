namespace Weather.SupportClasses
{
    public class WeatherParameters
    {
        public float temp { get; set; } //Temperature. In Celsius
        public float pressure { get; set; } //Atmospheric pressure (on the sea level, if there is no sea_level or grnd_level data), hPa
        public float humidity { get; set; } //Humidity, %
        public float temp_min { get; set; } //Minimum temperature at the moment. This is deviation from current temp that is possible for large cities and megalopolises geographically expanded (use these parameter optionally). In Celsius.
        public float temp_max { get; set; } //Maximum temperature at the moment. This is deviation from current temp that is possible for large cities and megalopolises geographically expanded (use these parameter optionally). In Celsius.
        public float sea_level { get; set; } //Atmospheric pressure on the sea level, hPa
        public float grnd_level { get; set; } //Atmospheric pressure on the ground level, hPa
    }
}
