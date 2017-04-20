using Nomnio.CityWeather.SupportClasses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nomnio.CityWeather.Interfaces
{
    public interface ICountry
    {
        Task<IEnumerable<Country>> GetAllCountryAndCapitalCityNamesAsync();
    }
}
