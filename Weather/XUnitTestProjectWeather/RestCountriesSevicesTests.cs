using System.Linq;
using System.Threading.Tasks;
using Nomnio.Weather;
using Xunit;

namespace WeatherUnitTests
{
    public class RestCountriesSevicesTests : TestBase
    {
        [Theory]
        [InlineData(250)]
        [InlineData(245)]   //This is the only right one
        public async Task NumberOfCountriesTest(int Expected)
        {
            var countries = await new RestCountriesSevices().GetAllCountriesWithCapitalCityNamesAsync();

            var result = countries.Count();

            Assert.Equal(Expected, result);
        }
    }
}
