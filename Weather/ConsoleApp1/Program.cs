using System;
using Nomnio.CityWeather.SupportClasses;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var t=TestMethod();
            
            Task.WhenAny(t);
            Console.ReadKey();
        }

        static async Task TestMethod()
        {
            var country = new Country();
            var countries = await country.GetAllCountryAndCapitalCityNamesAsync();
            var city = new City();
            var cities = await city.GetCapitalCityInformationAsync(countries);
            var cities2 = await city.GetCityWeatherAsync(countries);
            //var t=test.GetCityWeatherAsync("Ljubljana", "SI");
            
        }
    }
}