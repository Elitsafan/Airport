using Airport.Models.Enums;

namespace Airport.Models.Interfaces
{
    public interface IFlight
    {
        Guid FlightId { get; set; }
        FlightType FlightType { get; }
    }
}
