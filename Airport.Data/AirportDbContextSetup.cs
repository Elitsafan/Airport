using Airport.Data.Configurations;
using Airport.Models.Interfaces;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Airport.Data
{
    public class AirportDbContextSetup : IAirportDbContextSetup
    {
        #region Fields
        private bool _isConfigured;
        private readonly IMongoClient _client;
        private readonly ILogger<AirportDbContextSetup> _logger;
        private readonly IAirportDbConfiguration _configuration;
        #endregion

        public AirportDbContextSetup(
            IMongoClient client,
            ILogger<AirportDbContextSetup> logger,
            IAirportDbConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SeedDatabaseAsync()
        {
            try
            {
                if (_isConfigured)
                    return;
                await new DepartureConfiguration()
                    .ConfigureAsync(_client, _configuration);
                await new FlightConfiguration()
                    .ConfigureAsync(_client, _configuration);
                await new LandingConfiguration()
                    .ConfigureAsync(_client, _configuration);
                await new RouteConfiguration()
                    .ConfigureAsync(_client, _configuration);
                await new StationConfiguration()
                    .ConfigureAsync(_client, _configuration);
                await new TrafficLightConfiguration()
                    .ConfigureAsync(_client, _configuration);
                _isConfigured = true;
            }
            catch (Exception e)
            {
                await Task.FromException(e);
            }
        }
        public async Task DropDatabaseAsync()
        {
            try
            {
                await _client.DropDatabaseAsync(_configuration.DatabaseName);
            }
            catch (TimeoutException e)
            {
                _logger.LogError(null, e);
                await Task.FromException(e);
            }
            catch (Exception e)
            {
                _logger.LogError(null, e);
                await Task.FromException(e);
            }
        }
    }
}