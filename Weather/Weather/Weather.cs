using System;
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Table; // Namespace for Table storage types
using Microsoft.WindowsAzure.Storage.Auth;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;
using Weather.SupportClasses;
using Weather.WeatherTableEntity;
using Serilog;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Weather
{
    public class WeatherTable : IWeatherTable
    {
        private string AccountName { get; set; } = "mt1";
        private string KeyValue { get; set; } = "O9+FoFPCQ4wqqfMJLm5I1zp7sePAgGGfowvDmCnGBt+AKlrdTXGOJ8QuzoQWz7yTsKPiOvBRE/8PfW5kRzzsTg==";

        public WeatherTable()
        {
            Log.Logger = new LoggerConfiguration()
                    .WriteTo.RollingFile("log-{Date}.txt", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
                    .CreateLogger();
        }

        public WeatherTable(string accountName, string keyValue)
        {
            this.AccountName = accountName;
            this.KeyValue = keyValue;
            Log.Logger = new LoggerConfiguration()
                    .WriteTo.RollingFile("log-{Date}.txt", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
                    .CreateLogger();
        }

        public async Task WriteCityWeatherToTableAsync(City city)
        {
            bool useHttps = true;
            var storageCredentials = new StorageCredentials(AccountName, KeyValue);
            var storageAccount = new CloudStorageAccount(storageCredentials, useHttps);
            Log.Information("Connected to {Connection}", storageAccount);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference("WeatherTable");
            await table.CreateIfNotExistsAsync();
            await InsertOrReplaceIntoTable(table, city);
            Log.Information($"Wrote weather information into {table.Name}");
        }

        public async Task WriteCityWeatherToTableAsync(IEnumerable<City> cities)
        {
            bool useHttps = true;
            var storageCredentials = new StorageCredentials(AccountName, KeyValue);
            var storageAccount = new CloudStorageAccount(storageCredentials, useHttps);
            Log.Information("Connected to {Connection}", storageAccount);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference("WeatherTable");
            await table.CreateIfNotExistsAsync();
            foreach (var city in cities)
            {
                await InsertOrReplaceIntoTable(table, city);
            }
            Log.Information($"Wrote weather information into {table.Name}");
        }

        private async Task InsertOrReplaceIntoTable(CloudTable table, City city)
        {
            WeatherEntity cityEntity = new WeatherEntity(city);
            TableOperation insert = TableOperation.InsertOrReplace(cityEntity);
            await table.ExecuteAsync(insert);
            Log.Information($"Entity added. Orignal ETag = {cityEntity.ETag}");
        }

        //testna metoda
        public async Task FillWeatherTableForCapitalsOfCountriesAsync()
        {
            var countries = await GetAllCountryAndCapitalCityNamesAsync();       //Get an IEnumarable with countries and the names of their capitals
            var capitalCities = await GetCapitalCityInformationAsync(countries);    //Get information of all the capital cities of countries send into it with an IEnumarable
            //var f = await GetCityGroupIdWeatherAsync(capitalCities);
            //capitalCities = await GetCityWeatherAsync(capitalCities);               //Get weather information for the IEnumerable of cities

            var c = await GetCityWeatherAsync("Vienna", "AT");                         //Get weather information for the name of the city
            var d = await GetCityWeatherAsync(48.21f, 16.37f);                    //Get weather information for the city at the coordinates
            //await WriteCityWeatherToTableAsync(d);
            //await WriteCityWeatherToTableAsync(f);
            //await WriteCityWeatherToTableAsync(capitalCities);
        }

        public async Task<IEnumerable<Country>> GetAllCountryAndCapitalCityNamesAsync()
        {
            using (HttpClient clientCountries = new HttpClient())
            {
                List<string> errors = new List<string>();

                string URLCountries = "https://restcountries.eu/rest/v2/all";
                string urlParameters = "?fields=name;capital;altSpellings";
                IEnumerable<Country> countries = new List<Country>();
                clientCountries.BaseAddress = new Uri(URLCountries);
                clientCountries.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

                // List data responseCountries.
                HttpResponseMessage responseCountries = await clientCountries.GetAsync(urlParameters);
                if (responseCountries.IsSuccessStatusCode)
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings();

                    settings.NullValueHandling = NullValueHandling.Ignore;
                    settings.MissingMemberHandling = MissingMemberHandling.Ignore;
                    // used for error loging
                    settings.Error = delegate (object sender, ErrorEventArgs args)
                    {
                        errors.Add(args.ErrorContext.Error.Message);
                        args.ErrorContext.Handled = true;
                    };

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
                    Log.Information($"{(int)responseCountries.StatusCode} ({responseCountries.ReasonPhrase})");
                }

                if (errors.Count > 0)
                {
                    foreach (var ex in errors)
                    {
                        Log.Error(ex);
                    }
                }
                Log.Information("Downloaded country and capital city names.");
                return countries;
            }
        }

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
                            foreach (var item in rcvdData.Where(w => !string.IsNullOrEmpty(w.Capital) && w.altSpellings[0] == c.country && w.Capital.ToUpper() == c.Name.ToUpper()))
                            {
                                var city = new City();
                                city.id = item.id = int.Parse(c._id);
                                city.Name = c.Name;
                                city.coord = c.coord;
                                city.country = c.country;

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
                        cityIds = cityIds + obj.id + ",";
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
                                                int index = c.list.FindIndex(a => a.id == obj.id);
                                                if (index != -1)
                                                {
                                                    obj.weather = c.list[index].weather;
                                                    obj.main = c.list[index].main;
                                                    obj.country = c.list[index].sys.country;
                                                    obj.sys = c.list[index].sys;
                                                    obj.coord = c.list[index].coord;
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
                    var o = await GetCityWeatherAsync(obj.Name, obj.country);

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
                                        city.country = city.sys.country;
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
