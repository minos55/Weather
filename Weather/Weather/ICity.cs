using Nomnio.CityWeather;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nomnio.CityWeather.Interfaces
{
    public interface ICity
    {
        string Name { get; set; }
        int Id { get; set; }
        string Country { get; set; }

        Task<IEnumerable<City>> GetCapitalCityIDsAsync(IEnumerable<ICountry> countires);
        Task<IEnumerable<City>> GetCityWeatherWithIdsAsync(IEnumerable<ICity> cities);
        Task<IEnumerable<City>> GetCityWeatherAsync(IEnumerable<ICity> Cities);
        Task<IEnumerable<City>> GetCityWeatherAsync(IEnumerable<ICountry> countries);
        Task<City> GetCityWeatherAsync(string cityName);
        Task<City> GetCityWeatherAsync(string cityName, string country);
        Task<City> GetCityWeatherAsync(float lat, float lon);
    }
}
