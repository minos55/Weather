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
using JSonWeather;

namespace Weather
{
    public class WeatherTable
    {
        string accountName = "mt1";
        string keyValue = "O9+FoFPCQ4wqqfMJLm5I1zp7sePAgGGfowvDmCnGBt+AKlrdTXGOJ8QuzoQWz7yTsKPiOvBRE/8PfW5kRzzsTg==";

        private const string URLCountries = "https://restcountries.eu/rest/v2/all";
        private string urlParameters = "?fields=name;capital;altSpellings";
        private const string URLWeather = "http://api.openweathermap.org/data/2.5/group";
        string urlParametersWeather = "?id=**ID's**&units=metric&appid=dd40332c4190d0feb5adbeef17305957"; //**ID's** is later replaced with city id's

        public ICollection<City> CapitalCities { get; } = new List<City>();

        public WeatherTable()
        {
            //RunSamples().Wait();
            

        }
        private async Task RunSamples()
        {
            bool useHttps = true;
            bool exportSecrets = true;
            var storageCredentials = new StorageCredentials(accountName, keyValue);
            var storageAccount = new CloudStorageAccount(storageCredentials, useHttps);

            //var connString = storageAccount.ToString(exportSecrets);

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference("Weather");
            await table.CreateIfNotExistsAsync();
        }
        public void FillWeatherTableForCapitalsOfCountries()
        {
            GetCapitalCountriesAndCities();
            GetCapitalCityWeathers();
            Console.WriteLine("Downloaded weather parameters for capital cities.");
        }

        private void GetCapitalCountriesAndCities()
        {
            using (HttpClient clientCountries = new HttpClient())
            {
                clientCountries.BaseAddress = new Uri(URLCountries);
                clientCountries.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

                // List data responseCountries.
                HttpResponseMessage responseCountries = clientCountries.GetAsync(urlParameters).Result;  // Blocking call!
                if (responseCountries.IsSuccessStatusCode)
                {
                    // Parse the responseCountries body. Blocking!

                    JsonSerializerSettings settings = new JsonSerializerSettings();
                    settings.NullValueHandling = NullValueHandling.Ignore;
                    settings.MissingMemberHandling = MissingMemberHandling.Ignore;

                    using (var responseStreamCountries = responseCountries.Content.ReadAsStreamAsync().Result)
                    {
                        if (responseStreamCountries != null)
                        {
                            var countries = JArray.Parse(new StreamReader(responseStreamCountries).ReadToEnd());
                            GetCapitalCities(JsonConvert.DeserializeObject<IEnumerable<Country>>(countries.ToString(), settings));
                        }
                    }
                }
                else
                {
                    Console.WriteLine("{0} ({1})", (int)responseCountries.StatusCode, responseCountries.ReasonPhrase);
                }
            }
        }

        private void GetCapitalCities(IEnumerable<Country> rcvdData)
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\current_cities.json";
            if (File.Exists(path))
            {
                using (var stream = File.OpenRead(path))
                using (StreamReader streamReader = new StreamReader(stream))
                using (JsonTextReader jsonReader = new JsonTextReader(streamReader))
                {
                    jsonReader.SupportMultipleContent = true;

                    var serializer = new JsonSerializer();
                    while (jsonReader.Read())
                    {
                        if (jsonReader.TokenType == JsonToken.StartObject)
                        {
                            City c = serializer.Deserialize<City>(jsonReader);
                            foreach (var item in rcvdData.Where(w => !string.IsNullOrEmpty(w.Capital) && w.altSpellings[0] == c.country && w.Capital.ToUpper() == c.Name.ToUpper()))
                            {
                                var city = new City();
                                city.id = item.id = int.Parse(c._id);
                                city.Name = c.Name;
                                city.country = c.country;

                                //Console.WriteLine(item.id + "; "+ c._id + item.Capital + "; "+ c.Name + "; " +item.Name + "; " + c.country+"; "+ item.altSpellings[0]);

                                if (!CapitalCities.Contains(city))
                                {
                                    CapitalCities.Add(city);
                                }
                                else
                                {
                                    Console.WriteLine(item.Capital + "; " + item.Name);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void GetCapitalCityWeathers()
        {
            using (HttpClient clientWeather = new HttpClient())
            {
                clientWeather.BaseAddress = new Uri(URLWeather);
                clientWeather.DefaultRequestHeaders.Accept.Add(
    new MediaTypeWithQualityHeaderValue("application/json"));

                //Used for sending max 20 city id's
                string cityIds = "";

                for (int i = 0; i < CapitalCities.Count; i = i + 20)
                {
                    var items = CapitalCities.Skip(i).Take(20);
                    foreach (var obj in items)
                    {
                        cityIds = cityIds + obj.id + ",";
                    }
                    cityIds = cityIds.Remove(cityIds.Length - 1);

                    string urlParametersWeatherFixed = urlParametersWeather.Replace("**ID's**", cityIds);
                    var responseWeather = clientWeather.GetAsync(urlParametersWeatherFixed).Result;
                    if (responseWeather.IsSuccessStatusCode)
                    {
                        using (var responseStreamWeather = responseWeather.Content.ReadAsStreamAsync().Result)
                        {
                            if (responseStreamWeather != null)
                            {
                                using (StreamReader streamReader = new StreamReader(responseStreamWeather))
                                using (JsonTextReader jsonReader = new JsonTextReader(streamReader))
                                {
                                    jsonReader.SupportMultipleContent = true;

                                    var serializer = new JsonSerializer();
                                    while (jsonReader.Read())
                                    {
                                        if (jsonReader.TokenType == JsonToken.StartObject)
                                        {
                                            var c = serializer.Deserialize<RootObject>(jsonReader);
                                            foreach (var obj in CapitalCities)
                                            {
                                                int index = c.list.FindIndex(a => a.id == obj.id);
                                                if (index != -1)
                                                {
                                                    obj.weather = c.list[index].weather;
                                                    obj.main = c.list[index].main;
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
                        Console.WriteLine("{0} ({1})", (int)responseWeather.StatusCode, responseWeather.ReasonPhrase);
                    }
                    cityIds = "";
                }
            }
        }
    }


    public class CustomerEntity : TableEntity
    {
        public CustomerEntity(string lastName, string firstName)
        {
            this.PartitionKey = lastName;
            this.RowKey = firstName;
        }

        public CustomerEntity() { }



        public string Capacity { get; set; }
        public string ContainerCount { get; set; }
        public string ObjectCount { get; set; }
    }





}
