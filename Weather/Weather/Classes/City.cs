using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Nomnio.CityWeather.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Nomnio.CityWeather
{
    public class City : WeatherBase
    {
        public string Name { get; set; } = string.Empty; //Name of city
        [JsonProperty("_id")]//Id of city from restcountries.eu
        private int _id{get;set;}
        public int Id { get { return _id; } set { _id=value; } } //Id of city from api.openweathermap.org

        public string Country { get; set; } = string.Empty;//Code of country
        public Coordinates Coord { get; set; } = new Coordinates();
        public dynamic Sys { get; set; }
        public List<WeatherDescription> Weather { get; set; } = new List<WeatherDescription>();
        public WeatherParameters Main { get; set; } = new WeatherParameters();

        public City()
        {
            InitializeLogger();
        }

        public City(string _name, string _country)
        {
            InitializeLogger();
            Name = _name;
            Country = _country;
        }
    }
}
