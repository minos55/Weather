using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Nomnio.CityWeather.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Nomnio.CityWeather.SupportClasses
{
    public class City
    {
        public string Name { get; set; } = string.Empty; //Name of city
        public string _id { get; set; } = string.Empty;//Id of city from restcountries.eu
        public int Id { get; set; } //Id of city from api.openweathermap.org
        public string Country { get; set; } = string.Empty;//Code of country
        public Coordinates Coord { get; set; } = new Coordinates();
        public dynamic Sys { get; set; }
        public List<WeatherDescription> Weather { get; set; } = new List<WeatherDescription>();
        public WeatherParameters Main { get; set; } = new WeatherParameters();

        public async Task<IEnumerable<City>> GetCapitalCityInformationAsync(IEnumerable<Country> rcvdData)
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\current_cities.json";
            if (File.Exists(path))
            {
                List<string> errors = new List<string>();

                ICollection<City> CapitalCities = new List<City>();
                using (var stream = File.OpenRead(path))
                using (StreamReader streamReader = new StreamReader(stream))
                using (JsonTextReader jsonReader = new JsonTextReader(streamReader))
                {
                    jsonReader.SupportMultipleContent = true;

                    var serializer = new JsonSerializer();
                    serializer.Error += delegate (object sender, ErrorEventArgs args)
                    {
                        // used for error loging
                        if (args.CurrentObject == args.ErrorContext.OriginalObject)
                        {
                            errors.Add(args.ErrorContext.Error.Message);
                            args.ErrorContext.Handled = true;
                        }
                    };

                    while (await jsonReader.ReadAsync())
                    {
                        if (jsonReader.TokenType == JsonToken.StartObject)
                        {
                            City c = serializer.Deserialize<City>(jsonReader);
                            foreach (var item in rcvdData.Where(w => !string.IsNullOrEmpty(w.Capital) && w.AltSpellings.First() == c.Country && w.Capital.ToUpper() == c.Name.ToUpper()))
                            {
                                var city = new City();
                                city.Id = int.Parse(c._id);
                                city.Name = c.Name;
                                city.Coord = c.Coord;
                                city.Country = c.Country;

                                if (!CapitalCities.Contains(city))
                                {
                                    CapitalCities.Add(city);
                                }
                            }
                        }
                    }
                }

                if (errors.Count > 0)
                {
                    foreach (var ex in errors)
                    {
                        Log.Error(ex);
                    }
                }
                Log.Information("Downloaded information for capital cities.");
                return CapitalCities;
            }
            return new List<City>();
        }

        public async Task<IEnumerable<City>> GetCityGroupIdWeatherAsync(IEnumerable<City> Cities)
        {
            #region Faster way of geting weather information for more cities
            using (HttpClient clientWeather = new HttpClient())
            {
                List<string> errors = new List<string>();

                string URLWeather = "http://api.openweathermap.org/data/2.5/group";
                clientWeather.BaseAddress = new Uri(URLWeather);
                clientWeather.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

                //Used for sending max 20 city id's
                string cityIds = "";

                for (int i = 0; i < Cities.Count(); i = i + 20)
                {
                    var items = Cities.Skip(i).Take(20);
                    foreach (var obj in items)
                    {
                        cityIds = cityIds + obj.Id + ",";
                    }
                    cityIds = cityIds.Remove(cityIds.Length - 1);

                    string urlParametersWeather = $"?id={cityIds}&units=metric&appid=dd40332c4190d0feb5adbeef17305957";
                    var responseWeather = await clientWeather.GetAsync(urlParametersWeather);
                    if (responseWeather.IsSuccessStatusCode)
                    {
                        using (var responseStreamWeather = await responseWeather.Content.ReadAsStreamAsync())
                        {
                            if (responseStreamWeather != null)
                            {
                                using (StreamReader streamReader = new StreamReader(responseStreamWeather))
                                using (JsonTextReader jsonReader = new JsonTextReader(streamReader))
                                {
                                    jsonReader.SupportMultipleContent = true;

                                    var serializer = new JsonSerializer();
                                    serializer.Error += delegate (object sender, ErrorEventArgs args)
                                    {
                                        // used for error loging
                                        if (args.CurrentObject == args.ErrorContext.OriginalObject)
                                        {
                                            errors.Add(args.ErrorContext.Error.Message);
                                            args.ErrorContext.Handled = true;
                                        }
                                    };
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
                                                    obj.Weather = c.List[index].Weather;
                                                    obj.Main = c.List[index].Main;
                                                    obj.Country = c.List[index].Sys.country;
                                                    obj.Sys = c.List[index].Sys;
                                                    obj.Coord = c.List[index].Coord;
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
                        Log.Information($"{(int)responseWeather.StatusCode} ({responseWeather.ReasonPhrase})");
                    }
                    cityIds = "";
                }
                if (errors.Count > 0)
                {
                    foreach (var ex in errors)
                    {
                        Log.Error(ex);
                    }
                }
                Log.Information("Downloaded weather information for cities.");
                return Cities;
            }
            #endregion
        }

        public async Task<IEnumerable<City>> GetCityWeatherAsync(IEnumerable<City> Cities)
        {
            #region Slower way of geting weather information for more cities
            int requestsPerMinuteLimit = 50;
            ICollection<City> citiesList = new List<City>();
            for (int i = 0; i < Cities.Count(); i = i + requestsPerMinuteLimit)
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                var items = Cities.Skip(i).Take(requestsPerMinuteLimit);

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
            #region Slower way of geting weather information for more cities
            int requestsPerMinuteLimit = 50;
            ICollection<City> citiesList = new List<City>();
            var Cities = countries.Where(x => !string.IsNullOrWhiteSpace(x.Capital) && !string.IsNullOrWhiteSpace(x.AltSpellings.First()));
            for (int i = 0; i < Cities.Count(); i = i + requestsPerMinuteLimit)
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                var items = Cities.Skip(i).Take(requestsPerMinuteLimit);

                foreach (var obj in items)
                {
                    var o = await GetCityWeatherAsync(obj.Capital, obj.AltSpellings.First());

                    if (!citiesList.Contains(o))
                    {
                        citiesList.Add(o);
                    }


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

        public async Task<City> GetCityWeatherAsync(string cityName, string countryCode)
        {
            string urlParametersWeather = $"?q={cityName},{countryCode}&units=metric&appid=dd40332c4190d0feb5adbeef17305957";
            var obj = await GetWeatherAsync(urlParametersWeather);
            Log.Information($"Downloaded weather information for the city {cityName}.");
            return obj;
        }

        public async Task<City> GetCityWeatherAsync(string cityName)
        {
            string urlParametersWeather = $"?q={cityName}&units=metric&appid=dd40332c4190d0feb5adbeef17305957";
            var obj = await GetWeatherAsync(urlParametersWeather);
            Log.Information($"Downloaded weather information for the city {cityName}.");
            return obj;
        }

        public async Task<City> GetCityWeatherAsync(float lat, float lon)
        {
            string urlParametersWeather = $"?lat={lat}&lon={lon}&units=metric&appid=dd40332c4190d0feb5adbeef17305957";
            var obj = await GetWeatherAsync(urlParametersWeather);
            Log.Information($"Downloaded weather information for the city at lat={lat},lon={lon}.");
            return obj;
        }

        private async Task<City> GetWeatherAsync(string urlParametersWeather)
        {
            using (HttpClient clientWeather = new HttpClient())
            {
                List<string> errors = new List<string>();

                City city = new City();
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
                            using (StreamReader streamReader = new StreamReader(responseStreamWeather))
                            using (JsonTextReader jsonReader = new JsonTextReader(streamReader))
                            {
                                jsonReader.SupportMultipleContent = true;

                                var serializer = new JsonSerializer();
                                serializer.Error += delegate (object sender, ErrorEventArgs args)
                                {
                                    // used for error loging
                                    if (args.CurrentObject == args.ErrorContext.OriginalObject)
                                    {
                                        errors.Add(args.ErrorContext.Error.Message);
                                        args.ErrorContext.Handled = true;
                                    }
                                };
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
                    Log.Information($"{(int)responseWeather.StatusCode} ({responseWeather.ReasonPhrase})");
                }

                if (errors.Count > 0)
                {
                    foreach (var ex in errors)
                    {
                        Log.Error(ex);
                    }
                }
                return city;
            }
        }
    }
}
