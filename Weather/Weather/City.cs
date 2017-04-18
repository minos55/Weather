using System;
using System.Collections.Generic;

namespace Weather.SupportClasses
{
    public class City
    {
        public string Name { get; set; } = string.Empty; //Name of city
        public string _id { get; set; } = string.Empty;//Id of city from restcountries.eu
        public int id { get; set; } //Id of city from api.openweathermap.org
        public string country { get; set; } = string.Empty;//Code of country
        public Coordinates coord { get; set; } = new Coordinates();
        public dynamic sys { get; set; }
        public List<WeatherDescription> weather { get; set; } = new List<WeatherDescription>();
        public WeatherParameters main { get; set; } = new WeatherParameters();

        public static implicit operator int(City v)
        {
            throw new NotImplementedException();
        }
    }
}
