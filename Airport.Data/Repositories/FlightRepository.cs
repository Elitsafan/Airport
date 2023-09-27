using Airport.Models.Entities;
using Airport.Models.Interfaces;
using MongoDB.Driver;

namespace Airport.Data.Repositories
{
    public class FlightRepository : IFlightRepository
    {
        private readonly IMongoCollection<Flight> _flightsCollection;
        private IMongoClient? _client;

        public FlightRepository(IMongoClient client, IAirportDbConfiguration dbSettings)
        {
            _client = client;
            _flightsCollection = _client
                .GetDatabase(dbSettings.DatabaseName)
                .GetCollection<Flight>(dbSettings.FlightsCollectionName);
        }
        public async Task AddFlightAsync(Flight flight) => await _flightsCollection
            .InsertOneAsync(flight);
        public async Task<IEnumerable<Flight>> GetAllAsync() => await _flightsCollection
            .Find(Builders<Flight>.Filter.Empty)
            .ToListAsync();
        public async Task<IEnumerable<T>> OfTypeAsync<T>() where T : Flight => await _flightsCollection
            .OfType<T>()
            .Find(Builders<T>.Filter.Empty)
            .ToListAsync();
        public async Task<bool> UpdateFlightAsync(Flight flight)
        {
            try
            {
                UpdateResult updateResult = await _flightsCollection.UpdateOneAsync(
                    f => f.FlightId == flight.FlightId,
                    Builders<Flight>.Update
                        .Set(nameof(Flight.StationOccupationDetails), flight.StationOccupationDetails)
                        .Set(nameof(Flight.RouteId), flight.RouteId),
                    new UpdateOptions { IsUpsert = false });
                return updateResult.MatchedCount > 0;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public void Dispose() => _client = null;
    }
}
