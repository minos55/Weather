using System.Collections.Generic;
using System.Threading.Tasks;
using Weather.SupportClasses;

namespace Weather
{
    public interface IWeatherTable
    {
        Task WriteCityWeatherToTableAsync(City city);
        Task WriteCityWeatherToTableAsync(IEnumerable<City> cities);
        Task<IEnumerable<Country>> GetAllCountryAndCapitalCityNamesAsync();
        Task<IEnumerable<City>> GetCapitalCityInformationAsync(IEnumerable<Country> rcvdData);
        Task<IEnumerable<City>> GetCityGroupIdWeatherAsync(IEnumerable<City> CapitalCities);
        Task<IEnumerable<City>> GetCityWeatherAsync(IEnumerable<City> Cities);
        Task<City> GetCityWeatherAsync(string cityName);
        Task<City> GetCityWeatherAsync(string cityName, string countryCode);
        Task<City> GetCityWeatherAsync(float lat, float lon);
        Task FillWeatherTableForCapitalsOfCountriesAsync(); //testna
    }
}
