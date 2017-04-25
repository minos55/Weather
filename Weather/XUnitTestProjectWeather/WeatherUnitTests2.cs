using System;
using System.Linq;
using System.Threading.Tasks;
using Nomnio.Weather;
using Xunit;

namespace WeatherUnitTests
{
    public class WeatherUnitTests2 : TestBase
    {
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
        [InlineData(250)]
        [InlineData(245)]   //This is the only right one
        public async Task NumberOfCountriesTest(int Expected)
        {
            //Arrange

            //Act
            var countries = await new RestCountriesSevices().GetAllCountriesWithCapitalCityNamesAsync();

            var result = countries.Count();

            //Assert
            Assert.Equal(Expected, result);
        }
    }
}
