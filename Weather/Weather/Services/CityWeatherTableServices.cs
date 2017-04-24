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

        public async Task WriteCityWeatherToTableAsync(City city,string _tableName)
        {
            var storageAccount = GetCloudStorageAccount();

            var table = await GetTableAsync(storageAccount, _tableName);

            await InsertOrReplaceIntoTable(table, city);
            myLog.Information("Wrote weather information into {Table}", table.Name);
        }

        public async Task WriteCityWeatherToTableAsync(IEnumerable<City> cities, string _tableName)
        {
            var storageAccount = GetCloudStorageAccount();

            var table = await GetTableAsync(storageAccount, _tableName);
            foreach (var city in cities)
            {
                await InsertOrReplaceIntoTable(table, city);
            }
            myLog.Information("Wrote weather information into {Table}", table.Name);
        }

        private async Task InsertOrReplaceIntoTable(CloudTable table, City city)
        {
            var cityEntity = new CityWeatherTableEntity(city);
            var insert = TableOperation.InsertOrReplace(cityEntity);
            await table.ExecuteAsync(insert);
            myLog.Information("Entity added. Orignal ETag = {Entity}", cityEntity.ETag);
        }
    }
}
