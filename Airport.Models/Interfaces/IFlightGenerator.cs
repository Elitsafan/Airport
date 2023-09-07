using Airport.Models.Enums;

namespace Airport.Models.Interfaces
{
    public interface IFlightGenerator
    {
        /// <summary>
        /// Generates a single <see cref="IFlight"></see>/>
        /// </summary>
        /// <param name="flightType">Indicates if the flight is a landing or departure flight</param>
        /// <returns>An <see cref="IFlight"/> instance</returns>
        IFlight GenerateFlight(FlightType flightType);
        /// <summary>
        /// Generates a list of <see cref="IFlight"></see>/>
        /// </summary>
        /// <returns>The list of the generated flights</returns>
        IEnumerable<IFlight> GenerateFlights(int n = 6);
    }
}
