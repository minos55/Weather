using Microsoft.Extensions.Configuration;
using Nomnio.Weather;
using Nomnio.Weather.Interfaces;
using Serilog;
using System;
using System.IO;
using System.Linq;
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
            var countryService = new RestCountriesSevices();
            var countries = await countryService.GetAllCountriesWithCapitalCityNamesAsync();

            //var countries = new List<Country>();
            //countries.Add(new Country("Slovenia", "Ljubljana", "SI"));
            //countries.Add(new Country("Slovenia", "Maribor", "SI"));

            var weatherService = new OpenWeatherMapServices();
            var weatherStore = new AzureTableWeatherStore(connectionString, tableName);

            var tasks = countries.Select(country => GetAndStoreWeatherForCountry(weatherService, weatherStore, country)).ToArray();
            await Task.WhenAll(tasks);

        }

        static async Task GetAndStoreWeatherForCountry(IWeatherServices weatherService, IWeatherStore weatherStore, Country country)
        {
            var weather = await weatherService.GetWeatherAsync(country.CapitalCity, country.CountryCode);
            await weatherStore.Save(weather);
        }
    }
}
