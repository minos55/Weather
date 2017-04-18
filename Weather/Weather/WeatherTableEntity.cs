using Microsoft.WindowsAzure.Storage.Table;
using System.Linq;
using Weather.SupportClasses;

namespace Weather.WeatherTableEntity
{
    public class WeatherEntity : TableEntity
    {
        public WeatherEntity(string cityName, string countryName)
        {
            this.PartitionKey = cityName;
            this.RowKey = countryName;
        }
        public WeatherEntity(City city)
        {
            this.PartitionKey = city.Name;
            this.RowKey = city.country;
            this.Lat = city.coord.lat.ToString();
            this.Lon = city.coord.lon.ToString();
            this.WeatherParameter = city.weather.FirstOrDefault().main;
            this.WeatherDescription = city.weather.FirstOrDefault().description;
            this.Temp = city.main.temp.ToString();
            this.Pressure = city.main.pressure.ToString();
            this.Humidity = city.main.humidity.ToString();
            this.Temp_Min = city.main.temp_min.ToString();
            this.Temp_Max = city.main.temp_max.ToString();
            this.Sea_level = city.main.sea_level.ToString();
            this.Grnd_level = city.main.grnd_level.ToString();
        }

        public WeatherEntity(string cityName, string countryName, float lat, float lon, string weatherParameter, string weatherDescription, float temp, float pressure, float humidity, float temp_Min, float temp_Max, float sea_level, float grnd_level)
        {
            this.PartitionKey = cityName;
            this.RowKey = countryName;
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
