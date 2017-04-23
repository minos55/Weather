using Nomnio.CityWeather;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nomnio.CityWeather.Interfaces
{
    public interface IOpenWeatherMapServices
    {
        Task<IEnumerable<City>> GetCapitalCityIDsAsync(IEnumerable<Country> countires);
        Task<IEnumerable<City>> GetCityWeatherWithIdsAsync(IEnumerable<City> cities);
        Task<IEnumerable<City>> GetCityWeatherAsync(IEnumerable<City> Cities);
        Task<IEnumerable<City>> GetCityWeatherAsync(IEnumerable<Country> countries);
        Task<City> GetCityWeatherAsync(string cityName);
        Task<City> GetCityWeatherAsync(string cityName, string country);
        Task<City> GetCityWeatherAsync(float lat, float lon);
    }
}
