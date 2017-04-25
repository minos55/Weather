namespace Nomnio.Weather
{
    public class Weather
    {
        public string CityName { get; set; } = string.Empty;        //Name of city
        public string CountryCode { get; set; } = string.Empty;     //Code of country
        public float Lon { get; set; }
        public float Lat { get; set; }
        public string WeatherDescription { get; set; } = string.Empty;
        public float Temp { get; set; }

        public Weather()
        {}

        public Weather(string cityName, string countryCode)
        {
            CityName = cityName;
            CountryCode = countryCode;
        }

        public Weather(string cityName, string countryCode, float lon, float lat, string weatherDescription,float temp)
        {
            CityName = cityName;
            CountryCode = countryCode;
            Lon = lon;
            Lat = lat;
            WeatherDescription = weatherDescription;
            Temp = temp;
        }
    }
}
