using Newtonsoft.Json;
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
    public class OpenWeatherMapServices : IOpenWeatherMapServices
    {
        private const string apiKey = "&units=metric&appid=dd40332c4190d0feb5adbeef17305957";
        private const string informationString = "Downloaded weather information for the city";
        private ILogger myLog;
        private long timer;
        private int limitCounter;
        private DateTime startTime;

        public OpenWeatherMapServices()
        {
            myLog = Log.ForContext<OpenWeatherMapServices>();
            startTime = DateTime.Now;
        }

        public async Task<Weather> GetWeatherAsync(string cityName, string countryCode)
        {
            string urlParametersWeather = $"?q={cityName},{countryCode}{apiKey}";
            var obj = await GetWeatherAsync(urlParametersWeather);
            myLog.Information($"{informationString} {cityName}.");
            return obj;
        }

        public async Task<Weather> GetWeatherAsync(float lat, float lon)
        {
            string urlParametersWeather = $"?lat={lat}&lon={lon}{apiKey}";
            var obj = await GetWeatherAsync(urlParametersWeather);
            myLog.Information($"{informationString} at lat={lat},lon={lon}.");
            return obj;
        }

        private async Task<Weather> GetWeatherAsync(string urlParametersWeather)
        {
            //if this was called more then a minute ago reset counters
            if(startTime.AddMinutes(1) < DateTime.Now)
            {
                startTime = DateTime.Now;
                limitCounter = 0;
                timer = 0;
            }

            //if it was less then a minute run this
            if (limitCounter==50 && timer < 60000 )
            {
                limitCounter = 0;
                await Task.Delay(TimeSpan.FromMilliseconds(60000 - timer));
                startTime = DateTime.Now;
                timer = 0;
            }

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
                    var watch = System.Diagnostics.Stopwatch.StartNew();
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
                                        string name = obj.name;
                                        string country = obj.sys.country;
                                        float lon = obj.coord.lon;
                                        float lat = obj.coord.lat;
                                        IEnumerable<dynamic> dynamicWeather = obj.weather;
                                        string weatherDescription = dynamicWeather.ElementAt(0).main;
                                        float temp= obj.main.temp;
                                        weather = new Weather(name,country,lon,lat,weatherDescription,temp);
                                    }
                                }
                            }
                        }
                    }
                    watch.Stop();
                    var elapsedMs = watch.ElapsedMilliseconds;
                    limitCounter++;
                    timer += elapsedMs;
                }
                else
                {
                    myLog.Information("{StatusCode}({Reason})",(int)responseWeather.StatusCode, responseWeather.ReasonPhrase);
                }

                return weather;
            }
        }
    }
}
