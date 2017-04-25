using System.Threading.Tasks;

namespace Nomnio.Weather.Interfaces
{ 
    public interface IWeatherStore
    {
        Task Save(Weather weather);
    }
}
