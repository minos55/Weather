using Microsoft.WindowsAzure.Storage.Table;

namespace Nomnio.Weather
{
    public class WeatherTableEntity : TableEntity
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
        public string WeatherDescription { get; set; } = string.Empty;
        public double Temp { get; set; }

        public WeatherTableEntity()
        {
        }

        public WeatherTableEntity(string cityName, string countryCode)
        {
            PartitionKey = countryCode;
            RowKey = cityName;
        }

        public WeatherTableEntity(Weather weather)
        {
            PartitionKey = weather.CountryCode;
            RowKey = weather.CityName;
            Lat = weather.Lat;
            Lon = weather.Lon;
            WeatherDescription = weather.WeatherDescription;
            Temp = weather.Temp;
        }

        public WeatherTableEntity(string cityName, string countryCode, double lat, double lon, string weatherDescription, double temp)
        {
            PartitionKey = countryCode;
            RowKey = cityName;
            Lat = lat;
            Lon = lon;
            WeatherDescription = weatherDescription;
            Temp = temp;
        }
    }
}
