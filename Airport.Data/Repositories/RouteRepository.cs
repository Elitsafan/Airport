using Airport.Models.Entities;
using Airport.Models.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Airport.Data.Repositories
{
    public class RouteRepository : IRouteRepository
    {
        #region Fields
        private readonly IMongoCollection<Route> _routesCollection;
        private IMongoClient? _client;
        #endregion

        public RouteRepository(IMongoClient client, IAirportDbConfiguration dbSettings)
        {
            _client = client;
            _routesCollection = _client
                .GetDatabase(dbSettings.DatabaseName)
                .GetCollection<Route>(dbSettings.RoutesCollectionName);
        }

        public async Task<Route> GetRouteByIdAsync(ObjectId id) => await _routesCollection
            .Find(r => r.RouteId == id)
            .SingleAsync();
        public async Task<IEnumerable<Route>> GetAllAsync() => await _routesCollection
            .Find(Builders<Route>.Filter.Empty)
            .ToListAsync();
        public async Task<IEnumerable<Route>> GetRoutesByStationIdAsync(ObjectId stationId) => (await _routesCollection
            .Find(Builders<Route>.Filter.Empty)
            .ToListAsync())
            .Where(r => r.Directions
                .Select(d => new ObjectId[] { d.From, d.To })
                .SelectMany(arr => arr)
                .Any(id => id == stationId));
        public void Dispose() => _client = null;
    }
}
