using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Nomnio.CityWeather.Interfaces;

namespace Nomnio.CityWeather
{
    public class Country : WeatherBase
    {
        public string Name { get; set; } = string.Empty;                        //Country name
        public string Capital { get; set; } = string.Empty;                     //Capital name
        public List<string> AltSpellings { get; set; } = new List<string>();    //"EXAMPLE: altSpellings": ["CO", "Republic of Colombia", "República de Colombia"]

        public Country()
        {
            InitializeLogger();
        }
    }
}
