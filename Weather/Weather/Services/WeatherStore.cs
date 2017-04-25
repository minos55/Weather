using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;
using Nomnio.Weather.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Serilog;

namespace Nomnio.Weather
{
    public class WeatherStore : IWeatherStore
    {
        private ILogger myLog;
        private string ConnectionString;
        private string TableName;
        CloudStorageAccount storageAccount;

        public WeatherStore(string connectionString, string tableName)
        {
            myLog = Log.ForContext<WeatherStore>();
            ConnectionString = connectionString;
            TableName = tableName;
            storageAccount = GetCloudStorageAccount();
        }

        public async Task Save(Weather weather)
        {
            var table = await GetTableAsync();
            await InsertOrReplaceIntoTable(table, weather);
        }

        private async Task InsertOrReplaceIntoTable(CloudTable table, Weather weather)
        {
            var weatherEntity = new WeatherTableEntity(weather);
            var insert = TableOperation.InsertOrReplace(weatherEntity);
            await table.ExecuteAsync(insert);
            myLog.Information("Entity added. Orignal ETag = {Entity} into {Table}", weatherEntity.ETag, table.Name);
        }

        private async Task<CloudTable> GetTableAsync()
        {

            var sourceTableClient = storageAccount.CreateCloudTableClient();
            var table = sourceTableClient.GetTableReference(TableName);
            await table.CreateIfNotExistsAsync();
            return table;
        }

        private CloudStorageAccount GetCloudStorageAccount()
        {
            CloudStorageAccount StorageAccount = null;
            bool test = CloudStorageAccount.TryParse(ConnectionString, out StorageAccount);
            if (!test)
            {
                myLog.Error("Connection string is wrong.");
            }
            else
            {
                myLog.Information("Connected to {Connection}", storageAccount);
            }
            return StorageAccount;
        }
    }
}
