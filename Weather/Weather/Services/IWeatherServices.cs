using System.Threading.Tasks;

namespace Nomnio.Weather.Interfaces
{
    public interface IWeatherServices
    {
        Task<Weather> GetWeatherAsync(string cityName, string country);
        Task<Weather> GetWeatherAsync(double lat, double lon);
    }
}
