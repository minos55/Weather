using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nomnio.CityWeather.Interfaces
{ 
    public interface ICityWeatherTableServices
    {
        Task WriteCityWeatherToTableAsync(City city);
        Task WriteCityWeatherToTableAsync(IEnumerable<City> cities);
    }
}
