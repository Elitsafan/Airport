using Airport.Models.Entities;
using Airport.Models.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Airport.Data.Configurations
{
    internal class StationConfiguration
    {
        public async Task ConfigureAsync(IMongoClient client, IAirportDbConfiguration dbSettings)
        {
            var stationsCollection = client
                .GetDatabase(dbSettings.DatabaseName)
                .GetCollection<Station>(dbSettings.StationsCollectionName);
            var data = new List<Station>
            {
                new Station
                {
                    StationId = new ObjectId("000000000000000000000001"),
                    EstimatedWaitingTime = TimeSpan.FromMilliseconds(600),
                },
                new Station
                {
                    StationId = new ObjectId("000000000000000000000002"),
                    EstimatedWaitingTime = TimeSpan.FromMilliseconds(600),
                },
                new Station
                {
                    StationId = new ObjectId("000000000000000000000003"),
                    EstimatedWaitingTime = TimeSpan.FromMilliseconds(600),
                },
                new Station
                {
                    StationId = new ObjectId("000000000000000000000004"),
                    EstimatedWaitingTime = TimeSpan.FromMilliseconds(900),
                },
                new Station
                {
                    StationId = new ObjectId("000000000000000000000005"),
                    EstimatedWaitingTime = TimeSpan.FromMilliseconds(600),
                },
                new Station
                {
                    StationId = new ObjectId("000000000000000000000006"),
                    EstimatedWaitingTime = TimeSpan.FromMilliseconds(1200),
                },
                new Station
                {
                    StationId = new ObjectId("000000000000000000000007"),
                    EstimatedWaitingTime = TimeSpan.FromMilliseconds(1200),
                },
                new Station
                {
                    StationId = new ObjectId("000000000000000000000008"),
                    EstimatedWaitingTime = TimeSpan.FromMilliseconds(600),
                },
                new Station
                {
                    StationId = new ObjectId("000000000000000000000009"),
                    EstimatedWaitingTime = TimeSpan.FromMilliseconds(500),
                }
            };
            await stationsCollection.InsertManyAsync(data);
        }
    }
}