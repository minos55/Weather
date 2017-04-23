using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nomnio.CityWeather.Interfaces
{
    public interface IRestCountriesSevices
    {
        string Name { get; set; }
        string Capital { get; set; }
        List<string> AltSpellings { get; set; }
        Task<IEnumerable<Country>> GetAllCountriesAndCapitalCityNamesAsync();
    }
}
