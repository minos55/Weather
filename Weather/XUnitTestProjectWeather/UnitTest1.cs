using System;
using System.Linq;
using System.Threading.Tasks;
using Weather;
using Weather.SupportClasses;
using Xunit;

namespace XUnitTestProjectWeather
{
    public class UnitTest1
    {

        [Theory]
        [InlineData("Ljubljana","SI", 46.05f, 14.51f)]
        [InlineData("Athens", "GR", 37.98f, 23.72f)]
        [InlineData("Baghdad", "IQ", 33.34f, 44.4f)]
        
        public async Task UnitTestComparingOutputsTrue(string cityName,string countryCode,float lat,float lon)
        {
            //Arrange
            var test = new WeatherTable();

            var result = await test.GetCityWeatherAsync(lat,lon);
            var result2 = await test.GetCityWeatherAsync(cityName, countryCode);

            //Act
            var t = IsEqual(result, result2);

            //Assert
            Assert.True(t);

        }

        //Has to fail, even though the coordinates are the same as the city, the api returns diffrently called location if you call with coordinates.
        [Theory]
        [InlineData("Vienna", "AT", 48.21f, 16.37f)]
        [InlineData("Tokyo", "JP", 35.69f, 139.69f)]
        public async Task UnitTestComparingOutputsFalse(string cityName, string countryCode, float lat, float lon)
        {
            //Arrange
            var test = new WeatherTable();

            var result = await test.GetCityWeatherAsync(lat, lon);
            var result2 = await test.GetCityWeatherAsync(cityName, countryCode);

            //Act
            var t = IsEqual(result, result2);

            //Assert
            Assert.False(t);

        }

        [Theory]
        [InlineData(250)] //This is the only right one
        [InlineData(245)]
        [InlineData(202)]
        public async Task UnitTestNumberOfCountries(int Expected)
        {
            //Arrange
            var test = new WeatherTable();

            var result = await test.GetAllCountryAndCapitalCityNamesAsync();
            //Act
            

            //Assert
            Assert.Equal(Expected,result.Count());
        }
        [Theory]
        [InlineData(250)] 
        [InlineData(245)]   //This is the only right one
        [InlineData(202)]
        public async Task UnitTestNumberOfCountryCapitals(int Expected)
        {
            //Arrange
            var test = new WeatherTable();

            var result = await test.GetAllCountryAndCapitalCityNamesAsync();
            var cityResult = await test.GetCityWeatherAsync(result);
            //Act


            //Assert
            Assert.Equal(Expected, cityResult.Count());
        }

        bool IsEqual(City a, City b)
        {
            if (a.Name == b.Name && a._id == b._id && a.id == b.id && a.country == b.country && a.coord.lat == b.coord.lat && a.coord.lon == b.coord.lon &&
                a.weather.FirstOrDefault().main == b.weather.FirstOrDefault().main && a.weather.FirstOrDefault().description == b.weather.FirstOrDefault().description &&
                a.weather.FirstOrDefault().id == b.weather.FirstOrDefault().id && a.main.grnd_level == b.main.grnd_level && a.main.humidity == b.main.humidity &&
                a.main.pressure == b.main.pressure && a.main.sea_level == b.main.sea_level && a.main.temp == b.main.temp && a.main.temp_max == b.main.temp_max &&
                a.main.temp_min == b.main.temp_min )
            {
                return true;
            }
            return false;
        }

    }
}
