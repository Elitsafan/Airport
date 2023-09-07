using Airport.Models.Entities;

namespace Airport.Models.Interfaces
{
    public interface IFlightService
    {
        void ProcessFlight(Flight flight);
    }
}
