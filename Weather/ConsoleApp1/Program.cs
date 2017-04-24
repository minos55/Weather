using Nomnio.CityWeather;
using Serilog;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.LiterateConsole(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{SourceContext}] [{Level}] {Message}{NewLine}{Exception}")
                .WriteTo.RollingFile("log-{Date}.txt", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{SourceContext}] [{Level}] {Message}{NewLine}{Exception}")
                .CreateLogger();

            MainAsync(args).GetAwaiter().GetResult();

            Console.WriteLine("press any key");
            Console.ReadKey();
        }

        public static async Task MainAsync(string[] args)
        {
            await TestMethod();
        }

        static async Task TestMethod()
        {
            

            var country = new Country();
            
            var countries = await new RestCountriesSevices().GetAllCountriesAndCapitalCityNamesAsync();
            var city = new City();
            /* var cities = await city.GetCapitalCityIDsAsync(countries);
             var cities2 = await city.GetCityWeatherAsync(countries);
             var t=await city.GetCityWeatherAsync("Ljubljana", "SI");
            */
            var weatherService = new OpenWeatherMapServices();
            var citiesTask = await weatherService.GetCapitalCityIDsAsync(countries);
            var cities2Task = await weatherService.GetCityWeatherWithIdsAsync(citiesTask);
            var cities3Task = await weatherService.GetCityWeatherAsync(countries);
            var tTask = await weatherService.GetCityWeatherAsync("Ljubljana", "SI");

        }
    }
    }
