using Airport.Models.DTOs;
using Airport.Models.Interfaces;

namespace Airport.Services.Factories
{
    internal class LandingDTOFactory : IFlightDTOFactory
    {
        public IFlight Create() => new LandingDTO();
    }
}
