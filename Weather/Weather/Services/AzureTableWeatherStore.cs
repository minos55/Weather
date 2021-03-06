﻿using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;
using Nomnio.Weather.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Serilog;
using System;

namespace Nomnio.Weather
{
    public class AzureTableWeatherStore : IWeatherStore
    {
        private ILogger myLog;
        private string ConnectionString;
        private string TableName;
        CloudStorageAccount StorageAccount;

        public AzureTableWeatherStore(string connectionString, string tableName)
        {
            myLog = Log.ForContext<AzureTableWeatherStore>();
            ConnectionString = connectionString;
            TableName = tableName;
            StorageAccount = CloudStorageAccount.Parse(ConnectionString);
            myLog.Information("Connected to {Connection}", StorageAccount);
        }

        public async Task Save(Weather weather)
        {
            var table = await GetTableAsync();
            await InsertOrReplaceIntoTable(table, weather);
        }

        private async Task InsertOrReplaceIntoTable(CloudTable table, Weather weather)
        {
            
            if(!string.IsNullOrEmpty(weather.CityName)&&!string.IsNullOrEmpty(weather.CountryCode))
            {
                var weatherEntity = new WeatherTableEntity(weather);
                var insert = TableOperation.InsertOrReplace(weatherEntity);
                await table.ExecuteAsync(insert);
                myLog.Information("Entity added. Orignal ETag = {Entity} into {Table}", weatherEntity.ETag, table.Name);
            }
            else
            {
                throw new Exception("Weather not saved into table");
            }
        }

        private async Task<CloudTable> GetTableAsync()
        {
            var sourceTableClient = StorageAccount.CreateCloudTableClient();
            var table = sourceTableClient.GetTableReference(TableName);
            await table.CreateIfNotExistsAsync();
            return table;
        }
    }
}
