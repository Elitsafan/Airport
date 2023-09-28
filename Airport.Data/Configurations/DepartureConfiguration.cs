using Airport.Models.Interfaces;
using MongoDB.Driver;

namespace Airport.Data.Configurations
{
    internal class DepartureConfiguration
    {
        public async Task ConfigureAsync(IMongoClient client, IAirportDbConfiguration dbSettings) => await Task.CompletedTask;
    }
}