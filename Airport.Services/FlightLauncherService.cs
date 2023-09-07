using Airport.Models.Enums;
using Airport.Models.Helpers;
using Airport.Models.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Configuration;
using System.Net.Http.Json;

namespace Airport.Services
{
    public class FlightLauncherService : IFlightLauncherService
    {
        #region Fields
        private readonly Random _random;
        private readonly HttpClient _client;
        private readonly ILogger<FlightLauncherService> _logger;
        private readonly IFlightGenerator _flightGenerator;
        private readonly IConfiguration _configuration;
        private readonly FlightEndPointsConfiguration? _flightConfig;
        #endregion

        public FlightLauncherService(
            HttpClient client,
            ILogger<FlightLauncherService> logger,
            IFlightGenerator flightGenerator,
            IConfiguration configuration)
        {
            _random = new Random(DateTime.Now.Millisecond);
            _logger = logger;
            _client = client;
            _flightGenerator = flightGenerator;
            _configuration = configuration;
            // Gets the flight configuration to FlightEndPointsConfiguration instance
            _flightConfig = _configuration
                .GetSection("endpoints")
                .Get<FlightEndPointsConfiguration>();
            if (string.IsNullOrWhiteSpace(_flightConfig?.BaseUrl) ||
                string.IsNullOrWhiteSpace(_flightConfig?.Start) ||
                string.IsNullOrWhiteSpace(_flightConfig?.Departure) ||
                string.IsNullOrWhiteSpace(_flightConfig?.Landing))
                throw new ConfigurationErrorsException(
                    "Values for Start/AddFlight endpoints are missing.\n" +
                    "Please provide any in the configuration file and start again.");
            _client.BaseAddress = new Uri(_flightConfig.BaseUrl!);
        }
        // Launches multiple flights 
        public async Task<HttpResponseMessage[]> LaunchManyAsync()
        {
            var flights = _flightGenerator.GenerateFlights();
            return await Task.WhenAll(
                flights.Select(
                    flight =>
                    {
                        _logger.LogInformation($"Launching {flight.FlightType}...");
                        return flight.FlightType == FlightType.Landing
                        ? _client.PostAsJsonAsync(_flightConfig!.Landing, flight)
                        : _client.PostAsJsonAsync(_flightConfig!.Departure, flight);
                    })
                .ToList());
        }

        // Launches multiple flights 
        // Accepts args[0] is a number and args[1](optinal) is "exit"
        public async Task LaunchManyAsync(params string[]? args)
        {
            int numOfFlights = 0;
            // Input validation
            if (args is null ||
                !args.Any() ||
                !int.TryParse(args[0], out numOfFlights) ||
                numOfFlights <= 0)
                return;

            // Getnerates flights
            var flights = _flightGenerator.GenerateFlights(numOfFlights)
                .Select(f => LaunchOneAsync(f.FlightType))
                .ToArray();
            _logger.LogInformation($"Launching many flights...");
            await Task.WhenAll(flights);
            if (args![1] == "exit")
                Environment.Exit(0);
        }
        // Send a request to Start ep
        public Task<HttpResponseMessage> StartAsync() => _client.GetAsync(_flightConfig!.Start);
        public async Task<HttpResponseMessage> LaunchOneAsync(FlightType flightType)
        {
            _logger.LogInformation($"Launching {flightType}...");
            return flightType == FlightType.Landing
                ? await _client.PostAsJsonAsync(_flightConfig!.Landing, _flightGenerator.GenerateFlight(flightType))
                : await _client.PostAsJsonAsync(_flightConfig!.Departure, _flightGenerator.GenerateFlight(flightType));
        }
        // Launches a flight every 'timeout'
        public async Task SetFlightTimeoutAsync(TimeSpan timeout, FlightType? flightType)
        {
            var periodicTimer = new PeriodicTimer(timeout);
            while (await periodicTimer.WaitForNextTickAsync())
            {
                var result = await LaunchOneAsync(flightType ?? (_random.Next() % 2 == 0
                    ? FlightType.Landing
                    : FlightType.Departure));
                //_logger.LogInformation(result.ToString());
            }
        }
        public void Dispose() => _client?.Dispose();
    }
}
