using Airport.Models.Enums;
using Airport.Models.Interfaces;
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
        private readonly IFlightTimeoutConfiguration _flightTimeoutConfiguration;
        private readonly IFlightEndPointsConfiguration _flightsConfig;
        #endregion

        public FlightLauncherService(
            HttpClient client,
            ILogger<FlightLauncherService> logger,
            IFlightGenerator flightGenerator,
            IFlightTimeoutConfiguration flightTimeoutConfiguration,
            IFlightEndPointsConfiguration flightsConfiguration)
        {
            _random = new Random(DateTime.Now.Millisecond);
            _logger = logger;
            _client = client;
            _flightGenerator = flightGenerator;
            _flightTimeoutConfiguration = flightTimeoutConfiguration;
            _flightsConfig = flightsConfiguration;
            ValidateFlightsConfiguration();
            _client.BaseAddress = new Uri(_flightsConfig.BaseUrl!);
        }

        // Launches multiple flights 
        public async IAsyncEnumerable<HttpResponseMessage> LaunchMany()
        {
            var flights = _flightGenerator.GenerateFlights();
            Func<IFlight, Task<HttpResponseMessage>> task = async f =>
            {
                _logger.LogInformation($"Launching {f.FlightType}...");
                var flight = new { flight = f };
                return f.FlightType == FlightType.Landing
                    ? await _client.PostAsJsonAsync(_flightsConfig.Landing, flight)
                    : await _client.PostAsJsonAsync(_flightsConfig.Departure, flight);
            };
            foreach (var flight in flights
                .Select(flight => Task.Run(() => task(flight))))
                yield return await flight;
        }

        // Launches multiple flights 
        // Accepts args[0] is a number and args[1](optinal) is "exit"
        public async IAsyncEnumerable<HttpResponseMessage?> LaunchMany(params string[]? args)
        {
            int numOfFlights = 0;
            // Input validation
            if (args is null ||
                !args.Any() ||
                !int.TryParse(args[0], out numOfFlights) ||
                numOfFlights <= 0)
                yield return null;

            // Getnerates flights
            var flights = _flightGenerator.GenerateFlights(numOfFlights)
                .Select(f => Task.Run(async () => await LaunchOne(f.FlightType)))
                .ToArray();
            _logger.LogInformation($"Launching many flights...");
            foreach (var flight in flights)
                yield return await flight;
            if (args![1] == "exit")
                Environment.Exit(0);
        }
        // Send a request to Start ep
        public async Task<HttpResponseMessage> Start() => await _client.GetAsync(_flightsConfig.Start);
        public async Task<HttpResponseMessage> LaunchOne(FlightType flightType)
        {
            _logger.LogInformation($"Launching {flightType}...");
            var flight = new
            {
                flight = _flightGenerator.GenerateFlight(flightType)
            };
            return flightType == FlightType.Landing
                ? await _client.PostAsJsonAsync(_flightsConfig.Landing, flight)
                : await _client.PostAsJsonAsync(_flightsConfig.Departure, flight);
        }
        // Launches a flight according to _flightTimeoutConfiguration.Timeout
        public async Task SetFlightTimeout(FlightType? flightType)
        {
            var periodicTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(_flightTimeoutConfiguration.Timeout));
            while (await periodicTimer.WaitForNextTickAsync())
            {
                var result = await LaunchOne(flightType ?? (_random.Next() % 2 == 0
                    ? FlightType.Landing
                    : FlightType.Departure));
                //_logger.LogInformation(result.ToString());
            }
        }
        public void Dispose() => _client?.Dispose();

        private void ValidateFlightsConfiguration()
        {
            if (string.IsNullOrWhiteSpace(_flightsConfig?.BaseUrl) ||
                string.IsNullOrWhiteSpace(_flightsConfig?.Start) ||
                string.IsNullOrWhiteSpace(_flightsConfig?.Departure) ||
                string.IsNullOrWhiteSpace(_flightsConfig?.Landing))
                throw new ConfigurationErrorsException(
                    "Values for Start/AddFlight endpoints are missing.\n" +
                    "Please provide any in the configuration file and start again.");
        }
    }
}
