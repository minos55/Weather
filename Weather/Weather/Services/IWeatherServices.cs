using System.Threading.Tasks;

namespace Nomnio.Weather.Interfaces
{
    public interface IWeatherServices
    {
        Task<Weather> GetWeatherAsync(string cityName, string country);
        Task<Weather> GetWeatherAsync(float lat, float lon);
    }
}
