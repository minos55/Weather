using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nomnio.Weather.Interfaces
{
    public interface IRestCountriesSevices
    {
        Task<IEnumerable<Country>> GetAllCountriesWithCapitalCityNamesAsync();
    }
}
