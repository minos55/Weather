using System.Threading.Tasks;
using Nomnio.CityWeather;
using Xunit;
using System.Linq;
using System.Reflection;
using System.IO;

namespace WeatherUnitTests
{
    public class WeatherUnitTests : TestBase
    {

        [Theory]
        [InlineData("Ljubljana", "SI", 46.05f, 14.51f)]
        [InlineData("Athens", "GR", 37.98f, 23.72f)]
        [InlineData("Baghdad", "IQ", 33.34f, 44.4f)]

        public async Task ComparingOutputsTrueTest(string cityName, string countryCode, float lat, float lon)
        {
            //Arrange
            var city = new City();
            var weatherService = new OpenWeatherMapServices();
            //Act
            var result = await weatherService.GetCityWeatherAsync(lat, lon);
            var result2 = await weatherService.GetCityWeatherAsync(cityName, countryCode);
            var t = IsEqual(result, result2);

            //Assert
            Assert.True(t);

        }

        [Theory]
        [InlineData(250)] //This is the only right one
        [InlineData(245)]
        [InlineData(202)]
        public async Task UnitTestNumberOfCountries(int Expected)
        {
            //Arrange
            var country = new Country();

            //Act
            var result = await new RestCountriesSevices().GetAllCountriesAndCapitalCityNamesAsync();

            //Assert
            Assert.Equal(Expected, result.Count());
        }

        [Fact]
        public async Task WriteToTableTest()
        {
            string testTableName = RandomString(5);
            //Arrange
            var country = new Country();
            var city = new City();
            var weatherService = new OpenWeatherMapServices();
            var cityWeatherTable = new CityWeatherTableServices();
            var targetStorageAccount = GetCloudStorageAccount(connectionString);
            var targetTable = await GetTableAsync(targetStorageAccount, testTableName);

            //Act
            var countries = await new RestCountriesSevices().GetAllCountriesAndCapitalCityNamesAsync();

            var cities = await weatherService.GetCapitalCityIDsAsync(countries);

            var citiesWithWeather = await weatherService.GetCityWeatherWithIdsAsync(cities);

            await  cityWeatherTable.WriteCityWeatherToTableAsync(citiesWithWeather, testTableName);

            var enteties = await GetTableEntitiesAsync(targetTable);
            var expected = enteties.Count();

            await DeleteTableAsync(targetTable);
            
            int res = cities.Count();

            Assert.Equal(expected, res);

        }
    }
}
