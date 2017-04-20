using System;
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Table; // Namespace for Table storage types
using Microsoft.WindowsAzure.Storage.Auth;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;
using Nomnio.CityWeather.SupportClasses;
using Nomnio.CityWeather.WeatherTableEntity;
using Serilog;
using Newtonsoft.Json.Serialization;
using Nomnio.CityWeather.Interfaces;

namespace Nomnio.CityWeather.SupportClasses
{
    public class CityWeatherTable : ICityWeatherTable
    {
        private string AccountName { get; set; } = "mt1";
        private string KeyValue { get; set; } = "O9+FoFPCQ4wqqfMJLm5I1zp7sePAgGGfowvDmCnGBt+AKlrdTXGOJ8QuzoQWz7yTsKPiOvBRE/8PfW5kRzzsTg==";

        public CityWeatherTable()
        {
            Log.Logger = new LoggerConfiguration()
                    .WriteTo.RollingFile("log-{Date}.txt", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
                    .CreateLogger();
        }

        public CityWeatherTable(string accountName, string keyValue)
        {
            this.AccountName = accountName;
            this.KeyValue = keyValue;
            Log.Logger = new LoggerConfiguration()
                    .WriteTo.RollingFile("log-{Date}.txt", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
                    .CreateLogger();
        }

        public async Task WriteCityWeatherToTableAsync(City city)
        {
            bool useHttps = true;
            var storageCredentials = new StorageCredentials(AccountName, KeyValue);
            var storageAccount = new CloudStorageAccount(storageCredentials, useHttps);
            Log.Information("Connected to {Connection}", storageAccount);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference("WeatherTable");
            await table.CreateIfNotExistsAsync();
            await InsertOrReplaceIntoTable(table, city);
            Log.Information($"Wrote weather information into {table.Name}");
        }

        public async Task WriteCityWeatherToTableAsync(IEnumerable<City> cities)
        {
            bool useHttps = true;
            var storageCredentials = new StorageCredentials(AccountName, KeyValue);
            var storageAccount = new CloudStorageAccount(storageCredentials, useHttps);
            Log.Information("Connected to {Connection}", storageAccount);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference("WeatherTable");
            await table.CreateIfNotExistsAsync();
            foreach (var city in cities)
            {
                await InsertOrReplaceIntoTable(table, city);
            }
            Log.Information($"Wrote weather information into {table.Name}");
        }

        private async Task InsertOrReplaceIntoTable(CloudTable table, City city)
        {
            CityWeatherTableEntity cityEntity = new CityWeatherTableEntity(city);
            TableOperation insert = TableOperation.InsertOrReplace(cityEntity);
            await table.ExecuteAsync(insert);
            Log.Information($"Entity added. Orignal ETag = {cityEntity.ETag}");
        }

        

        
    }
}
