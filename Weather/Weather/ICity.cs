using Nomnio.CityWeather.SupportClasses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nomnio.CityWeather.Interfaces
{
    public interface ICity
    {
        Task<IEnumerable<ICity>> GetCapitalCityInformationAsync(IEnumerable<ICity> rcvdData);
        Task<IEnumerable<ICity>> GetCityGroupIdWeatherAsync(IEnumerable<ICity> CapitalCities);
        Task<IEnumerable<ICity>> GetCityWeatherAsync(IEnumerable<ICity> Cities);
        Task<IEnumerable<ICity>> GetCityWeatherAsync(IEnumerable<ICountry> countries);
        Task<ICity> GetCityWeatherAsync(string cityName);
        Task<ICity> GetCityWeatherAsync(string cityName, string countryCode);
        Task<ICity> GetCityWeatherAsync(float lat, float lon);
    }
}
