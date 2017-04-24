using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nomnio.CityWeather.Interfaces
{ 
    public interface ICityWeatherTableServices
    {
        Task WriteCityWeatherToTableAsync(City city, string _tableName);
        Task WriteCityWeatherToTableAsync(IEnumerable<City> cities, string _tableName);
    }
}
