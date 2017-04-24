using System;
using System.Linq;
using System.Threading.Tasks;
using Nomnio.CityWeather;
using Xunit;

namespace WeatherUnitTests
{
    public class WeatherUnitTests2 : TestBase
    {
        //Has to fail, even though the coordinates are the same as the city, the api returns diffrently called location if you call with coordinates.
        [Theory]
        [InlineData("Vienna", "AT", 48.21f, 16.37f)]
        [InlineData("Tokyo", "JP", 35.69f, 139.69f)]
        public async Task ComparingOutputsFalseTest(string cityName, string countryCode, float lat, float lon)
        {
            //Arrange
            var city = new City();
            var weatherService = new OpenWeatherMapServices();

            //Act
            var result = await weatherService.GetCityWeatherAsync(lat, lon);
            var result2 = await weatherService.GetCityWeatherAsync(cityName, countryCode);
            var t = IsEqual(result, result2);

            //Assert
            Assert.False(t);

        }

        [Theory]
        [InlineData(250)]
        [InlineData(245)]   //This is the only right one
        [InlineData(202)]
        public async Task NumberOfCountryCapitalsTest(int Expected)
        {
            //Arrange
            var country = new Country();
            var city = new City();

            //Act
            var result = await new RestCountriesSevices().GetAllCountriesAndCapitalCityNamesAsync();
            var cityResult = await new OpenWeatherMapServices().GetCityWeatherAsync(result);

            //Assert
            Assert.Equal(Expected, cityResult.Count());
        }
    }
}
