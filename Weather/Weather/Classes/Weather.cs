namespace Nomnio.Weather
{
    public class Weather
    {
        public string CityName { get; set; } = string.Empty;        //Name of city
        public string CountryCode { get; set; } = string.Empty;     //Code of country
        public double Lon { get; set; }
        public double Lat { get; set; }
        public string WeatherDescription { get; set; } = string.Empty;
        public double Temp { get; set; }

        public Weather()
        {}

        public Weather(string cityName, string countryCode)
        {
            CityName = cityName;
            CountryCode = countryCode;
        }

        public Weather(string cityName, string countryCode, double lon, double lat, string weatherDescription, double temp)
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
