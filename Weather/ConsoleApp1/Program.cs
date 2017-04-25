using Nomnio.Weather;
using Serilog;
using System;
using System.Collections.Generic;
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
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=mt1;AccountKey=O9+FoFPCQ4wqqfMJLm5I1zp7sePAgGGfowvDmCnGBt+AKlrdTXGOJ8QuzoQWz7yTsKPiOvBRE/8PfW5kRzzsTg==;EndpointSuffix=core.windows.net";
            string tableName = "WeatherTable2";

            var country = new Country();
            var countryService = new RestCountriesSevices();
            var countries = await countryService.GetAllCountriesWithCapitalCityNamesAsync();
            var weatherService = new OpenWeatherMapServices();
            List<Weather> test = new List<Weather>();
            foreach (var item in countries)
            {
                var weather = await weatherService.GetWeatherAsync(item.CapitalCity, item.CountryCode);
                test.Add(weather);
            }
            var weatherStore = new WeatherStore(connectionString, tableName);
            
            foreach(var item in test)
            {
                await weatherStore.Save(item);
            }

        }
    }
    }
