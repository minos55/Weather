using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nomnio.Weather.Interfaces
{
    public interface ICountriesSevices
    {
        Task<IEnumerable<Country>> GetAllCountriesWithCapitalCityNamesAsync();
    }
}
