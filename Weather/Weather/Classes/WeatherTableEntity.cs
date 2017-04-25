using Microsoft.WindowsAzure.Storage.Table;

namespace Nomnio.Weather
{
    public class WeatherTableEntity : TableEntity
    {

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
            Lat = weather.Lat.ToString();
            Lon = weather.Lon.ToString();
            WeatherDescription = weather.WeatherDescription;
            Temp = weather.Temp.ToString();
        }

        public WeatherTableEntity(string cityName, string countryCode, float lat, float lon, string weatherDescription, float temp)
        {
            PartitionKey = countryCode;
            RowKey = cityName;
            Lat = lat.ToString();
            Lon = lon.ToString();
            WeatherDescription = weatherDescription;
            Temp = temp.ToString();
        }

        public string Lat { get; set; } = string.Empty;
        public string Lon { get; set; } = string.Empty;
        public string WeatherDescription { get; set; } = string.Empty;
        public string Temp { get; set; } = string.Empty;
    }
}
