using Airport.Models.DTOs;
using Airport.Models.Interfaces;

namespace Airport.Services.Factories
{
    internal class DepartureDTOFactory : IFlightDTOFactory
    {
        public IFlight Create() => new DepartureDTO();
    }
}
