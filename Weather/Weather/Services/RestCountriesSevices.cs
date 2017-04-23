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
    public class RestCountriesSevices : WeatherBase, IRestCountriesSevices
    {
        public string Name { get; set; } = string.Empty;                        //Country name
        public string Capital { get; set; } = string.Empty;                     //Capital name
        public List<string> AltSpellings { get; set; } = new List<string>();    //"EXAMPLE: altSpellings": ["CO", "Republic of Colombia", "República de Colombia"]

        public RestCountriesSevices()
        {
            InitializeLogger();
        }
        public async Task<IEnumerable<Country>> GetAllCountriesAndCapitalCityNamesAsync()
        {
            using (var clientCountries = new HttpClient())
            {
                string URLCountries = "https://restcountries.eu/rest/v2/all";
                string urlParameters = "?fields=name;capital;altSpellings";
                IEnumerable<Country> countries = new List<Country>();
                clientCountries.BaseAddress = new Uri(URLCountries);
                clientCountries.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

                // List data responseCountries.
                var responseCountries = await clientCountries.GetAsync(urlParameters);
                if (responseCountries.IsSuccessStatusCode)
                {
                    var settings = new JsonSerializerSettings();

                    settings.NullValueHandling = NullValueHandling.Ignore;
                    settings.MissingMemberHandling = MissingMemberHandling.Ignore;
                    

                    using (var responseStreamCountries = await responseCountries.Content.ReadAsStreamAsync())
                    {
                        if (responseStreamCountries != null)
                        {
                            var obj = JArray.Parse(new StreamReader(responseStreamCountries).ReadToEnd());
                            countries = JsonConvert.DeserializeObject<IEnumerable<Country>>(obj.ToString(), settings);
                        }
                    }
                }
                else
                {
                    myLog.Information($"{(int)responseCountries.StatusCode} ({responseCountries.ReasonPhrase})");
                }

                LogError();
                myLog.Information("Downloaded country and capital city names.");
                return countries;
            }
        }
    }
}
