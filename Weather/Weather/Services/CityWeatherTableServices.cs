using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;
using System.Collections.Generic;
using Nomnio.CityWeather.Interfaces;

namespace Nomnio.CityWeather
{
    public class CityWeatherTableServices : WeatherBase, ICityWeatherTableServices
    {
        public CityWeatherTableServices()
        {
            InitializeLogger();
        }

        public async Task WriteCityWeatherToTableAsync(City city)
        {
            var storageAccount = GetCloudStorageAccount();

            var table = await GetTableAsync(storageAccount, tableName);

            await InsertOrReplaceIntoTable(table, city);
            myLog.Information($"Wrote weather information into {table.Name}");
        }

        public async Task WriteCityWeatherToTableAsync(IEnumerable<City> cities)
        {
            var storageAccount = GetCloudStorageAccount();

            var table = await GetTableAsync(storageAccount, tableName);
            foreach (var city in cities)
            {
                await InsertOrReplaceIntoTable(table, city);
            }
            myLog.Information($"Wrote weather information into {table.Name}");
        }

        private async Task InsertOrReplaceIntoTable(CloudTable table, City city)
        {
            var cityEntity = new CityWeatherTableEntity(city);
            var insert = TableOperation.InsertOrReplace(cityEntity);
            await table.ExecuteAsync(insert);
            myLog.Information($"Entity added. Orignal ETag = {cityEntity.ETag}");
        }
    }
}
