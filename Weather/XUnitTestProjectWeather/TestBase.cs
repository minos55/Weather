using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Nomnio.CityWeather;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherUnitTests
{
    public abstract class TestBase
    {
        protected const string connectionString = "DefaultEndpointsProtocol=https;AccountName=mt1;AccountKey=O9+FoFPCQ4wqqfMJLm5I1zp7sePAgGGfowvDmCnGBt+AKlrdTXGOJ8QuzoQWz7yTsKPiOvBRE/8PfW5kRzzsTg==;EndpointSuffix=core.windows.net";
        protected const string tableName = "WeatherTable";

        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        protected bool IsEqual(City a, City b)
        {
            if (a.Name == b.Name && a.Id == b.Id && a.Country == b.Country && a.Coord.Lat == b.Coord.Lat && a.Coord.Lon == b.Coord.Lon &&
                a.Weather.FirstOrDefault().Main == b.Weather.FirstOrDefault().Main && a.Weather.FirstOrDefault().Description == b.Weather.FirstOrDefault().Description &&
                a.Weather.FirstOrDefault().Id == b.Weather.FirstOrDefault().Id)
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

        protected async Task<IEnumerable<CityWeatherTableEntity>> GetTableEntitiesAsync(CloudTable table)
        {
            var tableQuery = new TableQuery<CityWeatherTableEntity>();

            IEnumerable<CityWeatherTableEntity> obj = new List<CityWeatherTableEntity>();
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
