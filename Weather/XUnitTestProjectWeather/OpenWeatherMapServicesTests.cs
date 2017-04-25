using System.Threading.Tasks;
using Nomnio.Weather;
using Xunit;

namespace WeatherUnitTests
{
    public class OpenWeatherMapServicesTests : TestBase
    {
        [Theory]
        [InlineData("Ljubljana", "SI", 46.05f, 14.51f)]
        [InlineData("Athens", "GR", 37.98f, 23.72f)]
        [InlineData("Baghdad", "IQ", 33.34f, 44.4f)]
        public async Task ComparingOutputsTrueTest(string cityName, string countryCode, float lat, float lon)
        {
            //Arrange
            var weatherService = new OpenWeatherMapServices();
            //Act
            var result = await weatherService.GetWeatherAsync(lat, lon);
            var result2 = await weatherService.GetWeatherAsync(cityName, countryCode);
            var t = IsEqual(result, result2);

            //Assert
            Assert.True(t);
        }

        //Has to be false, even though the coordinates are the same as the city, the api returns different location if you call with coordinates.
        [Theory]
        [InlineData("Vienna", "AT", 48.21f, 16.37f)]
        [InlineData("Tokyo", "JP", 35.69f, 139.69f)]
        public async Task ComparingOutputsFalseTest(string cityName, string countryCode, float lat, float lon)
        {
            //Arrange
            var weatherService = new OpenWeatherMapServices();

            //Act
            var result = await weatherService.GetWeatherAsync(lat, lon);
            var result2 = await weatherService.GetWeatherAsync(cityName, countryCode);
            var t = IsEqual(result, result2);

            //Assert
            Assert.False(t);
        }

        [Theory]
        [InlineData("Ljubljana", "SI",100)]
        public async Task TimeLimiterTest(string cityName, string countryCode,int iterations)
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
            var test = (elapsedMs > 60000*(iterations/50)) ? true:false;

            //Assert
            Assert.True(test);
        }

    }
}
