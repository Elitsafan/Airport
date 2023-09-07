using Airport.Models.Entities;
using Airport.Models.EventArgs;
using Airport.Models.Interfaces;
using Microsoft.Extensions.Logging;

namespace Airport.Services
{
    public class FlightService : IFlightService
    {
        private readonly ILogger<FlightService> _logger;
        private readonly IFlightLogicFactory _flightLogicFactory;

        public FlightService(IFlightLogicFactory flightLogicFactory, ILogger<FlightService> logger)
        {
            _flightLogicFactory = flightLogicFactory;
            _logger = logger;
        }

        public void ProcessFlight(Flight flight)
        {
            if (flight is null)
                throw new ArgumentNullException(nameof(flight));
            var flightLogic = _flightLogicFactory.Create(flight);
            flightLogic.FlightRunDone += OnFlightRunDoneAsync;
            // Starts the run
            _ = flightLogic.RunAsync();
        }
        // A handler to end a flight's run
        private Task OnFlightRunDoneAsync(object? sender, FlightRunDoneEventArgs e)
        {
            e.FlightDone.FlightRunDone -= OnFlightRunDoneAsync;
            e.FlightDone.Dispose();
            return Task.CompletedTask;
        }
    }
}
