using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Weather.IWeatherTable tess = new Weather.WeatherTable();
            tess.FillWeatherTableForCapitalsOfCountriesAsync().Wait();

            Console.WriteLine("Press any key to close.");
            Console.ReadKey();
        }
    }
}