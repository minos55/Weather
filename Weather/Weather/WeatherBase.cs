using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json.Serialization;
using Serilog;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Nomnio.CityWeather
{
    public abstract class WeatherBase
    {
       protected ILogger myLog;
        List<string> errors = new List<string>();
        protected const string apiKey = "&units=metric&appid=dd40332c4190d0feb5adbeef17305957";
        protected const string informationString = "Downloaded weather information for the city";
        protected const string connectionString = "DefaultEndpointsProtocol=https;AccountName=mt1;AccountKey=O9+FoFPCQ4wqqfMJLm5I1zp7sePAgGGfowvDmCnGBt+AKlrdTXGOJ8QuzoQWz7yTsKPiOvBRE/8PfW5kRzzsTg==;EndpointSuffix=core.windows.net";
        protected const string tableName = "WeatherTable";

        protected async Task<CloudTable> GetTableAsync(CloudStorageAccount storageAccount, string _tableName)
        {
            
            var sourceTableClient = storageAccount.CreateCloudTableClient();
            LogInformation("Connected to {Connection}", storageAccount);
            var table = sourceTableClient.GetTableReference(_tableName);
            await table.CreateIfNotExistsAsync();
            return table;
        }
        protected CloudStorageAccount GetCloudStorageAccount()
        {
            CloudStorageAccount StorageAccount = null;
            bool test = CloudStorageAccount.TryParse(connectionString, out StorageAccount);
            if (!test)
            {
                LogError("Connection string is wrong.");
            }
            return StorageAccount;
        }
        protected CloudTable GetTable(CloudStorageAccount storageAccount, string tableName)
        {
            var sourceTableClient = storageAccount.CreateCloudTableClient();
            

            return sourceTableClient.GetTableReference(tableName);
        }

        protected void InitializeLogger()
        {
            myLog = Log.ForContext(GetType());
        }

        protected void LogInformation(string information)
        {
            myLog.Information(information);
        }

        protected void LogInformation(string information,object t)
        {
            myLog.Information(information);
        }

        protected void LogError(string error)
        {
            myLog.Information(error);
        }

        protected void LogError()
        {
            if (errors.Count > 0)
            {
                foreach (var ex in errors)
                {
                    myLog.Error(ex);
                }
            }
            errors = new List<string>();
        }

        [OnError]
        internal void OnError(StreamingContext context, ErrorContext errorContext)
        {
            errors.Add(errorContext.Error.Message);
            errorContext.Handled = true;
        }

    }
}
