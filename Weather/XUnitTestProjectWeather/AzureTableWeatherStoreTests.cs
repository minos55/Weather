using System.Threading.Tasks;
using Nomnio.Weather;
using Xunit;
using System.Linq;

namespace WeatherUnitTests
{
    public class AzureTableWeatherStoreTests : TestBase
    {
        [Fact]
        public async Task WriteToTableTest()
        {
            string testTableName = RandomString(5);
            
            var weatherStore = new AzureTableWeatherStore(connectionString, testTableName);
            var targetStorageAccount = GetCloudStorageAccount(connectionString);
            var targetTable = await GetTableAsync(targetStorageAccount, testTableName);

            var cities = PrepareTestData();

            foreach (var item in cities)
            {
                await weatherStore.Save(item);
            }

            var expected = cities.Count();
            var enteties = await GetTableEntitiesAsync(targetTable);
            var result = enteties.Count();

            await DeleteTableAsync(targetTable);

            Assert.Equal(expected, result);
        }
    }
}
