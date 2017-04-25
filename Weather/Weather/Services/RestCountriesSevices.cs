using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nomnio.Weather.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Nomnio.Weather
{
    public class RestCountriesSevices : IRestCountriesSevices
    {
        private ILogger myLog;
        public RestCountriesSevices()
        {
            myLog = Log.ForContext<RestCountriesSevices>();
            
        }
        public async Task<IEnumerable<Country>> GetAllCountriesWithCapitalCityNamesAsync()
        {
            using (var clientCountries = new HttpClient())
            {
                string URLCountries = "https://restcountries.eu/rest/v2/all";
                string urlParameters = "?fields=name;capital;altSpellings";
                var countries = new List<Country>();
                clientCountries.BaseAddress = new Uri(URLCountries);
                clientCountries.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

                // List data responseCountries.
                var responseCountries = await clientCountries.GetAsync(urlParameters);
                if (responseCountries.IsSuccessStatusCode)
                {
                    using (var responseStreamCountries = await responseCountries.Content.ReadAsStreamAsync())
                    {
                        if (responseStreamCountries != null)
                        {
                            using (var streamReader = new StreamReader(responseStreamCountries))
                            using (var jsonReader = new JsonTextReader(streamReader))
                            {
                                jsonReader.SupportMultipleContent = true;

                                var serializer = new JsonSerializer();

                                while (await jsonReader.ReadAsync())
                                {
                                    if (jsonReader.TokenType == JsonToken.StartObject)
                                    {
                                        var obj = serializer.Deserialize<dynamic>(jsonReader);
                                        string countryName = obj.name;
                                        string capitalCity = obj.capital;
                                        IEnumerable<dynamic> AltSpellings = obj.altSpellings;
                                        string countryCode = AltSpellings.FirstOrDefault();
                                        if(!string.IsNullOrEmpty(countryName)&& !string.IsNullOrEmpty(capitalCity))
                                        {
                                            countries.Add(new Country(countryName, capitalCity, countryCode));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    myLog.Information($"{(int)responseCountries.StatusCode} ({responseCountries.ReasonPhrase})");
                }

                myLog.Information("Downloaded country and capital city names.");
                return countries;
            }
        }
    }
}
