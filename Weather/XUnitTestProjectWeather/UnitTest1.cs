using System;
using System.Linq;
using System.Threading.Tasks;
using Nomnio.CityWeather;
using Nomnio.CityWeather.SupportClasses;
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
            var city = new City();
            var test = new CityWeatherTable();

            var result = await city.GetCityWeatherAsync(lat,lon);
            var result2 = await city.GetCityWeatherAsync(cityName, countryCode);

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
            var city = new City();
            var test = new CityWeatherTable();

            var result = await city.GetCityWeatherAsync(lat, lon);
            var result2 = await city.GetCityWeatherAsync(cityName, countryCode);

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
            var country = new Country();
            var test = new CityWeatherTable();

            var result = await country.GetAllCountryAndCapitalCityNamesAsync();
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
            var country = new Country();
            var city = new City();
            var test = new CityWeatherTable();

            var result = await country.GetAllCountryAndCapitalCityNamesAsync();
            var cityResult = await city.GetCityWeatherAsync(result);
            //Act


            //Assert
            Assert.Equal(Expected, cityResult.Count());
        }

        bool IsEqual(City a, City b)
        {
            if (a.Name == b.Name && a._id == b._id && a.Id == b.Id && a.Country == b.Country && a.Coord.Lat == b.Coord.Lat && a.Coord.Lon == b.Coord.Lon &&
                a.Weather.FirstOrDefault().Main == b.Weather.FirstOrDefault().Main && a.Weather.FirstOrDefault().Description == b.Weather.FirstOrDefault().Description &&
                a.Weather.FirstOrDefault().Id == b.Weather.FirstOrDefault().Id && a.Main.Grnd_level == b.Main.Grnd_level && a.Main.Humidity == b.Main.Humidity &&
                a.Main.Pressure == b.Main.Pressure && a.Main.Sea_level == b.Main.Sea_level && a.Main.Temp == b.Main.Temp && a.Main.Temp_max == b.Main.Temp_max &&
                a.Main.Temp_min == b.Main.Temp_min )
            {
                return true;
            }
            return false;
        }

    }
}
