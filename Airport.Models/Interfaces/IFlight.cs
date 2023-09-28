using Airport.Models.Enums;
using MongoDB.Bson;

namespace Airport.Models.Interfaces
{
    public interface IFlight
    {
        ObjectId FlightId { get; set; }
        FlightType FlightType { get; }
    }
}
