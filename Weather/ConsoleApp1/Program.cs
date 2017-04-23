using Nomnio.CityWeather;
using Serilog;
using System;
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
            var countries = await country.GetAllCountriesAndCapitalCityNamesAsync();
            var city = new City();
            /* var cities = await city.GetCapitalCityIDsAsync(countries);
             var cities2 = await city.GetCityWeatherAsync(countries);
             var t=await city.GetCityWeatherAsync("Ljubljana", "SI");
 */
            var citiesTask = city.GetCapitalCityIDsAsync(countries);
            //var cities2Task = city.GetCityWeatherWithIdsAsync(citiesTask);
            var cities2Task = city.GetCityWeatherAsync(countries);
            var tTask = city.GetCityWeatherAsync("Ljubljana", "SI");

            await Task.WhenAll(citiesTask,cities2Task, tTask);

        }
    }
    }
