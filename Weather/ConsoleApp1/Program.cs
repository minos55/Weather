using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Weather.IWeatherTable tess = new Weather.WeatherTable();
            tess.FillWeatherTableForCapitalsOfCountriesAsync().Wait();
            var countries = tess.GetAllCountryAndCapitalCityNamesAsync().Result;       //Get an IEnumarable with countries and the names of their capitals
            var capitalCities = tess.GetCapitalCityInformationAsync(countries).Result;    //Get information of all the capital cities of countries send into it with an IEnumarable
            Console.WriteLine("Press any key to close.");
            Console.ReadKey();
        }
    }
}