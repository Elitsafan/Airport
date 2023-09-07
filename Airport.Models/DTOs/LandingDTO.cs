using Airport.Models.Enums;
using Airport.Models.Interfaces;

namespace Airport.Models.DTOs
{
    public class LandingDTO : IFlight
    {
        public Guid FlightId { get; set; }
        public FlightType FlightType => FlightType.Landing;
    }
}
