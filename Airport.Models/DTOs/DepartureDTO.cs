using Airport.Models.Enums;
using Airport.Models.Interfaces;

namespace Airport.Models.DTOs
{
    public class DepartureDTO : IFlight
    {
        public Guid FlightId { get; set; }
        public FlightType FlightType => FlightType.Departure;
    }
}
