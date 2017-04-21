using System.Threading.Tasks;
using Nomnio.CityWeather;
using Xunit;
using System.Linq;

namespace WeatherUnitTests
{
    public class WeatherUnitTests : TestBase
    {

        //[Theory]
        //[InlineData("Ljubljana", "SI", 46.05f, 14.51f)]
        //[InlineData("Athens", "GR", 37.98f, 23.72f)]
        //[InlineData("Baghdad", "IQ", 33.34f, 44.4f)]
        
        //public async Task ComparingOutputsTrueTest(string cityName,string countryCode,float lat,float lon)
        //{
        //    //Arrange
        //    var city = new City();

        //    //Act
        //    var result = await city.GetCityWeatherAsync(lat, lon);
        //    var result2 = await city.GetCityWeatherAsync(cityName, countryCode);
        //    var t = IsEqual(result, result2);

        //    //Assert
        //    Assert.True(t);

        //}

        //[Theory]
        //[InlineData(250)] //This is the only right one
        //[InlineData(245)]
        //[InlineData(202)]
        //public async Task UnitTestNumberOfCountries(int Expected)
        //{
        //    //Arrange
        //    var country = new Country();

        //    //Act
        //    var result = await country.GetAllCountriesAndCapitalCityNamesAsync();

        //    //Assert
        //    Assert.Equal(Expected, result.Count());
        //}

        [Fact]
        public async Task WriteToTableTest()
        {
            var SourceStorageAccount = GetCloudStorageAccount(connectionString);
            var SourceTable = await GetTableAsync(SourceStorageAccount, tableName);
            //Arrange
            var country = new Country();
            var city = new City();
            var cityweathertable = new CityWeatherTable();

            //Act
            var countries = await country.GetAllCountriesAndCapitalCityNamesAsync();

            var cities = await city.GetCapitalCityIDsAsync(countries);
            //var result = await  city.GetCityWeatherAsync(cities);


            int res = cities.Count();

            Assert.Equal(250, res);

        }
    }
}
