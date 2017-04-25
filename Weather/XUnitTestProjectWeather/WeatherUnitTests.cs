using System.Threading.Tasks;
using Nomnio.Weather;
using Xunit;
using System.Linq;
using System.Collections.Generic;

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
            var city = new Weather();
            var weatherService = new OpenWeatherMapServices();
            //Act
            var result = await weatherService.GetWeatherAsync(lat, lon);
            var result2 = await weatherService.GetWeatherAsync(cityName, countryCode);
            var t = IsEqual(result, result2);

            //Assert
            Assert.True(t);

        }

        [Fact]
        public async Task WriteToTableTest()
        {
            string testTableName = RandomString(5);
            var countryService = new RestCountriesSevices();
            var weatherService = new OpenWeatherMapServices();
            List<Weather> cities = new List<Weather>();

            var weatherStore = new WeatherStore(connectionString, testTableName);
            var targetStorageAccount = GetCloudStorageAccount(connectionString);
            var targetTable = await GetTableAsync(targetStorageAccount, testTableName);

            var countries = await countryService.GetAllCountriesWithCapitalCityNamesAsync();
            
            foreach (var item in countries)
            {
                var weather = await weatherService.GetWeatherAsync(item.CapitalCity, item.CountryCode);
                cities.Add(weather);
            }
            
            foreach (var item in cities)
            {
                await weatherStore.Save(item);
            }

            var expected = cities.Count;
            var enteties = await GetTableEntitiesAsync(targetTable);
            var result = enteties.Count();

            await DeleteTableAsync(targetTable);

            Assert.Equal(expected, result);

        }
    }
}
