using Airport.Models.DTOs;
using Airport.Models.Enums;
using Airport.Models.Interfaces;

namespace Airport.Services.Helpers
{
    public class FlightGenerator : IFlightGenerator
    {
        // Generates a single flight
        private IFlight GenerateFlight(FlightType flightType) => flightType == FlightType.Landing
            ? new LandingDTO()
            : new DepartureDTO();
        // Generates a list of flights
        public IEnumerable<IFlight> GenerateFlights(int n)
        {
            List<IFlight> flights = new();
            for (int i = 1; i <= n; i++)
                flights.Add(GenerateFlight(i % 2 == 0 ? FlightType.Departure : FlightType.Landing));
            return flights;
        }
        IFlight IFlightGenerator.GenerateFlight(FlightType flightType) => GenerateFlight(flightType);
    }
}
