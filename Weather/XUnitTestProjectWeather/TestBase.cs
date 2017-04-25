using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Nomnio.Weather;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherUnitTests
{
    public abstract class TestBase
    {
        protected const string connectionString = "DefaultEndpointsProtocol=https;AccountName=mt1;AccountKey=O9+FoFPCQ4wqqfMJLm5I1zp7sePAgGGfowvDmCnGBt+AKlrdTXGOJ8QuzoQWz7yTsKPiOvBRE/8PfW5kRzzsTg==;EndpointSuffix=core.windows.net";
        protected const string tableName = "WeatherTableTest";

        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        protected bool IsEqual(Weather a, Weather b)
        {
            if (a.CityName == b.CityName && a.CountryCode == b.CountryCode && a.Lat == b.Lat && a.Lon == b.Lon)
            {
                return true;
            }
            return false;
        }

        

        protected string RandomString(int length)
        {
            var random = new Random();
            string tableName = new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            return "t" + tableName;
        }

        protected async Task DeleteTableAsync(CloudTable table)
        {
            await table.DeleteIfExistsAsync();
        }

        protected async Task<IEnumerable<WeatherTableEntity>> GetTableEntitiesAsync(CloudTable table)
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

        protected CloudStorageAccount GetCloudStorageAccount(string connectionString)
        {
            CloudStorageAccount StorageAccount = null;
            CloudStorageAccount.TryParse(connectionString, out StorageAccount);

            return StorageAccount;
        }

        protected async Task<CloudTable> GetTableAsync(CloudStorageAccount storageAccount, string tableName)
        {
            var sourceTableClient = storageAccount.CreateCloudTableClient();
            var table = sourceTableClient.GetTableReference(tableName);
            await table.CreateIfNotExistsAsync();
            return table;
        }

        protected IEnumerable<Weather> PrepareTestData()
        {
            var testData = new List<Weather>();
            var random = new Random();
            int size = random.Next(200, 300);
            for (int i = 0; i < size; i++)
            {
                string cityName = new string(Enumerable.Repeat(chars, random.Next(1, 10))
              .Select(s => s[random.Next(s.Length)]).ToArray());
                string countryName = new string(Enumerable.Repeat(chars, random.Next(1, 3))
              .Select(s => s[random.Next(s.Length)]).ToArray());
                string weatherDescription = new string(Enumerable.Repeat(chars, random.Next(1, 16))
              .Select(s => s[random.Next(s.Length)]).ToArray());
                float temp = (float)random.NextDouble();
                testData.Add(new Weather(cityName, countryName,temp,temp, weatherDescription,temp));
            }

            return testData;
        }
    }
}
