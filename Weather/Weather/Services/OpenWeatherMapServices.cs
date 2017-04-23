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
    public class OpenWeatherMapServices : WeatherBase, IOpenWeatherMapServices
    {
        public OpenWeatherMapServices()
        {
            InitializeLogger();
        }

        public async Task<IEnumerable<City>> GetCapitalCityIDsAsync(IEnumerable<Country> countries)
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\current_cities.json";
            if (File.Exists(path))
            {
                var CapitalCities = new List<City>();
                using (var stream = File.OpenRead(path))
                using (var streamReader = new StreamReader(stream))
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    jsonReader.SupportMultipleContent = true;

                    var serializer = new JsonSerializer();

                    while (await jsonReader.ReadAsync())
                    {
                        if (jsonReader.TokenType == JsonToken.StartObject)
                        {
                            var c = serializer.Deserialize<City>(jsonReader);

                            foreach (var item in countries.Where(w => !string.IsNullOrEmpty(w.Capital) && w.AltSpellings.FirstOrDefault() == c.Country && w.Capital.ToUpper() == c.Name.ToUpper()))
                            {
                                if (!CapitalCities.Contains(c))
                                {
                                    CapitalCities.Add(c);
                                }
                            }
                        }
                    }
                }

                LogError();
                myLog.Information("Downloaded information {CitisCount }for capital cities. ");
                return CapitalCities;
            }
            return new List<City>();
        }

        public async Task<IEnumerable<City>> GetCityWeatherWithIdsAsync(IEnumerable<City> Cities)
        {
            #region Faster way of geting weather information for more cities
            using (var clientWeather = new HttpClient())
            {
                string URLWeather = "http://api.openweathermap.org/data/2.5/group";
                clientWeather.BaseAddress = new Uri(URLWeather);
                clientWeather.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

                //Used for sending max 20 city id's
                string cityIds = "";
                var _cities = new List<City>();

                for (int i = 0; i < Cities.Count(); i = i + 20)
                {
                    var items = Cities.Skip(i).Take(20);
                    foreach (var obj in items)
                    {
                        cityIds = cityIds + obj.Id + ",";
                    }
                    cityIds = cityIds.Remove(cityIds.Length - 1);

                    string urlParametersWeather = $"?id={cityIds}{apiKey}";
                    var responseWeather = await clientWeather.GetAsync(urlParametersWeather);
                    if (responseWeather.IsSuccessStatusCode)
                    {
                        using (var responseStreamWeather = await responseWeather.Content.ReadAsStreamAsync())
                        {
                            if (responseStreamWeather != null)
                            {
                                using (var streamReader = new StreamReader(responseStreamWeather))
                                using (var jsonReader = new JsonTextReader(streamReader))
                                {
                                    jsonReader.SupportMultipleContent = true;

                                    var serializer = new JsonSerializer();

                                    while (await jsonReader.ReadAsync())
                                    {
                                        if (jsonReader.TokenType == JsonToken.StartObject)
                                        {
                                            var c = serializer.Deserialize<RootObject>(jsonReader);
                                            foreach (var obj in Cities)
                                            {
                                                int index = c.List.FindIndex(a => a.Id == obj.Id);
                                                if (index != -1)
                                                {
                                                    _cities.Add(c.List[index]);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        myLog.Information($"{(int)responseWeather.StatusCode} ({responseWeather.ReasonPhrase})");
                    }
                    cityIds = "";
                }
                LogError();
                myLog.Information("Downloaded weather information for cities.");
                return _cities;
            }
            #endregion
        }

        public async Task<IEnumerable<City>> GetCityWeatherAsync(IEnumerable<City> cities)
        {
            #region Slower way of geting weather information for more cities
            int requestsPerMinuteLimit = 50;
            var citiesList = new List<City>();
            for (int i = 0; i < cities.Count(); i = i + requestsPerMinuteLimit)
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                var items = cities.Skip(i).Take(requestsPerMinuteLimit);

                foreach (var obj in items)
                {
                    var o = await GetCityWeatherAsync(obj.Name, obj.Country);

                    citiesList.Add(o);
                }
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;

                //if the requests finish sooner then 1 minute, the task is delayed till 1 minute is over so that new requests can be started
                if (elapsedMs < 60000)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(60000 - elapsedMs));
                }
            }
            return citiesList;
            #endregion
        }

        public async Task<IEnumerable<City>> GetCityWeatherAsync(IEnumerable<Country> countries)
        {
            var cities = new List<City>();
            var city = countries.Where(x => !string.IsNullOrWhiteSpace(x.Capital) && !string.IsNullOrWhiteSpace(x.AltSpellings.FirstOrDefault()));

            foreach (var item in city)
            {
                cities.Add(new City(item.Capital, item.AltSpellings.First()));
            }

            return await GetCityWeatherAsync(cities);

        }

        public async Task<City> GetCityWeatherAsync(string cityName, string country)
        {
            string urlParametersWeather = $"?q={cityName},{country}{apiKey}";
            var obj = await GetWeatherAsync(urlParametersWeather);
            myLog.Information($"{informationString} {cityName}.");
            return obj;
        }

        public async Task<City> GetCityWeatherAsync(string cityName)
        {
            string urlParametersWeather = $"?q={cityName}{apiKey}";
            var obj = await GetWeatherAsync(urlParametersWeather);
            myLog.Information($"{informationString} {cityName}.");
            return obj;
        }

        public async Task<City> GetCityWeatherAsync(float lat, float lon)
        {
            string urlParametersWeather = $"?lat={lat}&lon={lon}{apiKey}";
            var obj = await GetWeatherAsync(urlParametersWeather);
            myLog.Information($"{informationString} at lat={lat},lon={lon}.");
            return obj;
        }

        private async Task<City> GetWeatherAsync(string urlParametersWeather)
        {
            using (var clientWeather = new HttpClient())
            {
                var city = new City();
                string URLWeather = $"http://api.openweathermap.org/data/2.5/weather";
                clientWeather.BaseAddress = new Uri(URLWeather);
                clientWeather.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

                var responseWeather = await clientWeather.GetAsync(urlParametersWeather);
                if (responseWeather.IsSuccessStatusCode)
                {
                    using (var responseStreamWeather = await responseWeather.Content.ReadAsStreamAsync())
                    {
                        if (responseStreamWeather != null)
                        {
                            using (var streamReader = new StreamReader(responseStreamWeather))
                            using (var jsonReader = new JsonTextReader(streamReader))
                            {
                                jsonReader.SupportMultipleContent = true;

                                var serializer = new JsonSerializer();

                                while (await jsonReader.ReadAsync())
                                {
                                    if (jsonReader.TokenType == JsonToken.StartObject)
                                    {
                                        city = serializer.Deserialize<City>(jsonReader);
                                        city.Country = city.Sys.country;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    myLog.Information($"{(int)responseWeather.StatusCode} ({responseWeather.ReasonPhrase})");
                }

                LogError();
                return city;
            }
        }
    }
}
