using Microsoft.WindowsAzure.Storage.Table;
using System.Linq;
using Nomnio.CityWeather.SupportClasses;
using Nomnio.CityWeather.Interfaces;

namespace Nomnio.CityWeather.WeatherTableEntity
{
    public class CityWeatherTableEntity : TableEntity
    {
        public CityWeatherTableEntity(string cityName, string countryName)
        {
            this.PartitionKey = countryName;
            this.RowKey = cityName;
        }
        public CityWeatherTableEntity(City city)
        {
            this.PartitionKey = city.Country;
            this.RowKey = city.Name;
            this.Lat = city.Coord.Lat.ToString();
            this.Lon = city.Coord.Lon.ToString();
            this.WeatherParameter = city.Weather.FirstOrDefault().Main;
            this.WeatherDescription = city.Weather.FirstOrDefault().Description;
            this.Temp = city.Main.Temp.ToString();
            this.Pressure = city.Main.Pressure.ToString();
            this.Humidity = city.Main.Humidity.ToString();
            this.Temp_Min = city.Main.Temp_min.ToString();
            this.Temp_Max = city.Main.Temp_max.ToString();
            this.Sea_level = city.Main.Sea_level.ToString();
            this.Grnd_level = city.Main.Grnd_level.ToString();
        }

        public CityWeatherTableEntity(string cityName, string countryName, float lat, float lon, string weatherParameter, string weatherDescription, float temp, float pressure, float humidity, float temp_Min, float temp_Max, float sea_level, float grnd_level)
        {
            this.PartitionKey = countryName;
            this.RowKey = cityName;
            this.Lat = lat.ToString();
            this.Lon = lon.ToString();
            this.WeatherParameter = weatherParameter;
            this.WeatherDescription = weatherDescription;
            this.Temp = temp.ToString();
            this.Pressure = pressure.ToString();
            this.Humidity = humidity.ToString();
            this.Temp_Min = temp_Min.ToString();
            this.Temp_Max = temp_Max.ToString();
            this.Sea_level = sea_level.ToString();
            this.Grnd_level = grnd_level.ToString();
        }

        public string Lat { get; set; } = string.Empty;
        public string Lon { get; set; } = string.Empty;
        public string WeatherParameter { get; set; } = string.Empty;
        public string WeatherDescription { get; set; } = string.Empty;
        public string Temp { get; set; } = string.Empty;
        public string Pressure { get; set; } = string.Empty;
        public string Humidity { get; set; } = string.Empty;
        public string Temp_Min { get; set; } = string.Empty;
        public string Temp_Max { get; set; } = string.Empty;
        public string Sea_level { get; set; } = string.Empty;
        public string Grnd_level { get; set; } = string.Empty;
    }
}
