using System;
using System.Collections.Generic;
using System.Text;

namespace JSonWeather
{
    public class RootObject
    {
        public string Cnt { get; set; }
        public List<City> list { get; set; }
    }

    public class City
    {
        public string Name { get; set; } //Name of city
        public string _id { get; set; } //Id of city from restcountries.eu
        public int id { get; set; } //Id of city from api.openweathermap.org
        public string country { get; set; } //Code of country
        public List<WeatherDescription> weather { get; set; }
        public WeatherParameters main { get; set; }
    }

    public class Country
    {
        public string Name { get; set; } //Country name
        public string Capital { get; set; } //Capital name
        public List<string> altSpellings { get; set; } //"altSpellings": ["CO", "Republic of Colombia", "República de Colombia"],
        public int id { get; set; } //Id of capital city
    }

    public class WeatherDescription
    {
        public string main { get; set; } //weather parameters (Rain, Snow, Extreme etc.)
        public string id { get; set; } //Weather condition id
        public string description { get; set; } //Weather condition within the main parameter
    }

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
