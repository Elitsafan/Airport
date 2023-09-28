using Airport.Models.Enums;
using Airport.Models.Interfaces;
using MongoDB.Bson;

namespace Airport.Models.DTOs
{
    public class LandingDTO : IFlight
    {
        public ObjectId FlightId { get; set; }
        public FlightType FlightType => FlightType.Landing;
    }
}
