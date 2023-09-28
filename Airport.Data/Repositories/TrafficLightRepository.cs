using Airport.Models.Entities;
using Airport.Models.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Airport.Data.Repositories
{
    public class TrafficLightRepository : ITrafficLightRepository
    {
        #region Fields
        private readonly IAirportDbConfiguration _dbSettings;
        private readonly IMongoCollection<TrafficLight> _trafficLightsCollection;
        private IMongoClient? _client; 
        #endregion

        public TrafficLightRepository(IMongoClient client, IAirportDbConfiguration dbSettings)
        {
            _client = client;
            _dbSettings = dbSettings;
            _trafficLightsCollection = _client
                .GetDatabase(dbSettings.DatabaseName)
                .GetCollection<TrafficLight>(dbSettings.TrafficLightsCollectionName);
        }

        public async Task<IEnumerable<TrafficLight>> GetAllAsync() => await _trafficLightsCollection
            .Find(Builders<TrafficLight>.Filter.Empty)
            .ToListAsync();

        public async Task<IEnumerable<TrafficLight>> GetTrafficLightsByRouteIdAsync(ObjectId routeId)
        {
            var routesCollection = _client!
                .GetDatabase(_dbSettings.DatabaseName)
                .GetCollection<Route>(_dbSettings.RoutesCollectionName);
            var stationIds = (await routesCollection
                .Find(r => r.RouteId == routeId)
                .SingleAsync())
                .Directions
                .SelectMany(d => new ObjectId[] { d.From, d.To })
                .Distinct();

            return await _trafficLightsCollection
                .Find(Builders<TrafficLight>.Filter.In(x => x.StationId, stationIds))
                .ToListAsync();
        }
        public void Dispose() => _client = null;
    }
}
