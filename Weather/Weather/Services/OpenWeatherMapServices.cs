using Newtonsoft.Json;
using Nomnio.Weather.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Nomnio.Weather
{
    public class OpenWeatherMapServices : IWeatherServices
    {
        private const string apiKey = "&units=metric&appid=dd40332c4190d0feb5adbeef17305957";
        private const string informationString = "Downloaded weather information for the city";
        private ILogger myLog;

        private readonly int maxRequests = 50;
        
        private readonly TimeSpan _maxPeriod = new TimeSpan(0, 0, 1, 0);
        Throttler throttle;

        private readonly Queue<DateTime> _requestTimes = new Queue<DateTime>();

        public OpenWeatherMapServices()
        {
            myLog = Log.ForContext<OpenWeatherMapServices>();
            throttle = new Throttler(maxRequests, _maxPeriod);
        }

        public async Task<Weather> GetWeatherAsync(string cityName, string countryCode)
        {

            string urlParametersWeather = $"?q={cityName},{countryCode}{apiKey}";
            var obj = await throttle.Queue(GetWeatherAsync, urlParametersWeather);
            
            //if city not found try again with no spaces in name
            var weather = await obj;
            if(weather.CityName.Length==0)
            {
                cityName=Regex.Replace(cityName, @"\s+", "");
                urlParametersWeather = $"?q={cityName},{countryCode}{apiKey}";
                obj=await throttle.Queue(GetWeatherAsync, urlParametersWeather);

                weather = await obj;
                //if still not found throw exception
                if (weather.CityName.Length == 0)
                {
                    throw new Exception("City not found");
                }
            }

            myLog.Information($"{informationString} {cityName}.");
            return await obj;
        }

        public async Task<Weather> GetWeatherAsync(double lat, double lon)
        {
            string urlParametersWeather = $"?lat={lat}&lon={lon}{apiKey}";
            var obj = await throttle.Queue(GetWeatherAsync, urlParametersWeather);
            var weather = await obj;
            //if not found throw exception
            if (weather.CityName.Length == 0)
            {
                throw new Exception("City not found");
            }
            myLog.Information($"{informationString} at lat={lat},lon={lon}.");
            return await obj;
        }

        private async Task<Weather> GetWeatherAsync(string urlParametersWeather)
        {
            using (var clientWeather = new HttpClient())
            {
                var weather = new Weather();
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
                                        var obj= serializer.Deserialize<dynamic>(jsonReader);
                                        weather.CityName = obj.name;
                                        weather.CountryCode = obj.sys.country;
                                        weather.Lon = obj.coord.lon;
                                        weather.Lat = obj.coord.lat;
                                        IEnumerable<dynamic> dynamicWeather = obj.weather;
                                        weather.WeatherDescription = dynamicWeather.ElementAt(0).main;
                                        weather.Temp= obj.main.temp;
                                    }
                                }
                            }
                        }
                    }

                }
                return weather;
            }
        }
    }
}
