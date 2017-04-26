using System.Threading.Tasks;
using Nomnio.Weather;
using Xunit;

namespace WeatherUnitTests
{
    public class OpenWeatherMapServicesTests
    {
        [Theory]
        [InlineData("Ljubljana", "SI", 46.05f, 14.51f,true)]
        [InlineData("Athens", "GR", 37.98f, 23.72f, true)]
        [InlineData("Baghdad", "IQ", 33.34f, 44.4f, true)]
        [InlineData("Vienna", "AT", 48.21f, 16.37f,false)]
        [InlineData("Tokyo", "JP", 35.69f, 139.69f,false)]
        public async Task ComparingIfSearchByCityNameAndCountryIsSameAsWithCoordinates(string cityName, string countryCode, float lat, float lon,bool expected)
        {
            //Arrange
            var weatherService = new OpenWeatherMapServices();
            //Act
            var result = await weatherService.GetWeatherAsync(lat, lon);
            var result2 = await weatherService.GetWeatherAsync(cityName, countryCode);
            var test = IsEqual(result, result2);

            //Assert
            Assert.Equal(expected, test);
        }

        [Theory]
        [InlineData("Ljubljana", "SI",51,true)]
        [InlineData("Ljubljana", "SI", 50,false)]
        [InlineData("Ljubljana", "SI", 10,true)]
        public async Task GetWeatherThrottlerTest(string cityName, string countryCode,int iterations,bool expected)
        {
            
            //Arrange
            var weatherService = new OpenWeatherMapServices();

            //Act
            //for each 50 iterations it should wait till 1 minute is over, so after 100 iterations it should be atleast 2 minutes
            var watch = System.Diagnostics.Stopwatch.StartNew();
            for (int i=0;i<iterations;i++)
            {
                await weatherService.GetWeatherAsync(cityName, countryCode);
            }
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            var t = 60000 * (iterations / 50);
            var test = (elapsedMs > t) ? true:false;

            //Assert
            Assert.Equal(expected, test);
        }

        private bool IsEqual(Weather a, Weather b)
        {
            if (a.CityName == b.CityName && a.CountryCode == b.CountryCode && a.Lat == b.Lat && a.Lon == b.Lon)
            {
                return true;
            }
            return false;
        }

    }
}
