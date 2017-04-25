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
    }
}
