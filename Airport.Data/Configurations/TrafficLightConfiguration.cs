using Airport.Models.Entities;
using Airport.Models.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Airport.Data.Configurations
{
    internal class TrafficLightConfiguration
    {
        public async Task ConfigureAsync(IMongoClient client, IAirportDbConfiguration dbSettings)
        {
            var trafficLightsCollection = client
                .GetDatabase(dbSettings.DatabaseName)
                .GetCollection<TrafficLight>(dbSettings.TrafficLightsCollectionName);
            var data = new List<TrafficLight>
            {
                new TrafficLight
                {
                    TrafficLightId = ObjectId.GenerateNewId(),
                    StationId = new ObjectId("000000000000000000000004"),
                },
                new TrafficLight
                {
                    TrafficLightId = ObjectId.GenerateNewId(),
                    StationId = new ObjectId("000000000000000000000006"),
                },
                new TrafficLight
                {
                    TrafficLightId = ObjectId.GenerateNewId(),
                    StationId = new ObjectId("000000000000000000000007"),
                }
            };
            await trafficLightsCollection.InsertManyAsync(data);
        }
    }
}
