using System.Threading.Tasks;
using Nomnio.Weather;
using Xunit;
using System.Linq;
using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;

namespace WeatherUnitTests
{
    public class AzureTableWeatherStoreTests
    {
        [Theory]
        [InlineData(100)]
        public async Task WriteToTableTest(int numberOfTestData)
        {
            string testTableName = "T" + Guid.NewGuid().ToString().Substring(0,5);
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=mt1;AccountKey=O9+FoFPCQ4wqqfMJLm5I1zp7sePAgGGfowvDmCnGBt+AKlrdTXGOJ8QuzoQWz7yTsKPiOvBRE/8PfW5kRzzsTg==;EndpointSuffix=core.windows.net";
            var weatherStore = new AzureTableWeatherStore(connectionString, testTableName);
            var targetStorageAccount = GetCloudStorageAccount(connectionString);
            var targetTable = await GetTableAsync(targetStorageAccount, testTableName);

            var cities = PrepareTestData(numberOfTestData);

            foreach (var item in cities)
            {
                await weatherStore.Save(item);
            }

            var enteties = await GetTableEntitiesAsync(targetTable);
            var result = enteties.Count();

            await DeleteTableAsync(targetTable);

            Assert.Equal(numberOfTestData, result);
        }

        private IEnumerable<Weather> PrepareTestData(int numberOfTestData)
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var testData = new List<Weather>();
            var random = new Random();
            for (int i = 0; i < numberOfTestData; i++)
            {
                string cityName = new string(Enumerable.Repeat(chars, random.Next(1, 10))
              .Select(s => s[random.Next(s.Length)]).ToArray());
                string countryName = new string(Enumerable.Repeat(chars, random.Next(1, 3))
              .Select(s => s[random.Next(s.Length)]).ToArray());
                string weatherDescription = new string(Enumerable.Repeat(chars, random.Next(1, 16))
              .Select(s => s[random.Next(s.Length)]).ToArray());
                double temp = random.NextDouble();
                testData.Add(new Weather(cityName, countryName, temp, temp, weatherDescription, temp));
            }

            return testData;
        }

        private async Task DeleteTableAsync(CloudTable table)
        {
            await table.DeleteIfExistsAsync();
        }

        private async Task<IEnumerable<WeatherTableEntity>> GetTableEntitiesAsync(CloudTable table)
        {
            var tableQuery = new TableQuery<WeatherTableEntity>();

            IEnumerable<WeatherTableEntity> obj = new List<WeatherTableEntity>();
            TableContinuationToken continuationToken = null;
            do
            {
                // Retrieve a segment (up to 100 entities).
                var tableQueryResult = await table.ExecuteQuerySegmentedAsync(tableQuery, continuationToken);

                continuationToken = tableQueryResult.ContinuationToken;

                obj = tableQueryResult.Results;

            } while (continuationToken != null);
            return obj;
        }

        private CloudStorageAccount GetCloudStorageAccount(string connectionString)
        {
            CloudStorageAccount StorageAccount = null;
            CloudStorageAccount.TryParse(connectionString, out StorageAccount);

            return StorageAccount;
        }

        private async Task<CloudTable> GetTableAsync(CloudStorageAccount storageAccount, string tableName)
        {
            var sourceTableClient = storageAccount.CreateCloudTableClient();
            var table = sourceTableClient.GetTableReference(tableName);
            await table.CreateIfNotExistsAsync();
            return table;
        }
    }
}
