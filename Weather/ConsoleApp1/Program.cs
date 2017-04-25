using Microsoft.Extensions.Configuration;
using Nomnio.Weather;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        public static IConfigurationRoot Configuration { get; set; }
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
            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

            Configuration = builder.Build();

            string ConnectionString;
            string TableName;
            if (args.Length == 2)
            { 
                ConnectionString = args[0];
                TableName = args[1];
            }
            else
            {
                ConnectionString = Configuration["ConnectionString"];
                TableName = Configuration["TableName"];
            }

            await CallAllServices(ConnectionString, TableName); 
        }

        static async Task CallAllServices(string connectionString, string tableName)
        {
            var country = new Country();
            var countryService = new RestCountriesSevices();
            var countries = await countryService.GetAllCountriesWithCapitalCityNamesAsync();
            var weatherService = new OpenWeatherMapServices();
            ConcurrentBag<Country> CountryList = new ConcurrentBag<Country>(countries);
            ConcurrentBag<Weather> weatherList = new ConcurrentBag<Weather>();
            //List<Weather> weatherList = new List<Weather>();
            //foreach (var item in countries)
            //{
            //    var weather = await weatherService.GetWeatherAsync(item.CapitalCity, item.CountryCode);
            //    weatherList.Add(weather);
            //}
            Parallel.ForEach(CountryList, async (item) =>
            {
                var weather = await weatherService.GetWeatherAsync(item.CapitalCity, item.CountryCode);
                weatherList.Add(weather);
            });

            var weatherStore = new AzureTableWeatherStore(connectionString, tableName);

            Parallel.ForEach(weatherList, async (item) =>
             {
                 await weatherStore.Save(item);
             });
            //foreach (var item in weatherList)
            //{

            //}

        }
    }
    }
