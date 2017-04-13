using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Weather.WeatherTable tess=new Weather.WeatherTable();
            tess.FillWeatherTableForCapitalsOfCountries();

            Console.WriteLine("Press any key to close.");
            Console.ReadKey();
        }
    }
}