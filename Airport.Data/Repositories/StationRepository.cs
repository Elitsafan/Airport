using Airport.Models.Entities;
using Airport.Models.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Airport.Data.Repositories
{
    public class StationRepository : IStationRepository
    {
        private readonly IMongoCollection<Station> _stationsCollection;
        private readonly IMongoCollection<Route> _routesCollection;
        private IMongoClient? _client;

        public StationRepository(IMongoClient client, IAirportDbConfiguration dbSettings)
        {
            _client = client;
            _stationsCollection = _client
                .GetDatabase(dbSettings.DatabaseName)
                .GetCollection<Station>(dbSettings.StationsCollectionName);
            _routesCollection = _client
                .GetDatabase(dbSettings.DatabaseName)
                .GetCollection<Route>(dbSettings.RoutesCollectionName);
        }

        public async Task<IEnumerable<Station>> GetAllAsync() => await _stationsCollection
            .Find(Builders<Station>.Filter.Empty)
            .ToListAsync();
        public async Task<IEnumerable<Station>> GetStationsByRouteIdAsync(ObjectId routeId)
        {
            var route = await _routesCollection
                .Find(r => r.RouteId == routeId)
                .SingleAsync();
            var stationIds = route.Directions
                .Select(d => new ObjectId[] { d.From, d.To })
                .SelectMany(arr => arr)
                .Distinct();
            var filter = Builders<Station>.Filter.In(nameof(Station.StationId), stationIds);
            return await _stationsCollection
                .Find(filter)
                .ToListAsync();
        }
        public void Dispose() => _client = null;
    }
}
