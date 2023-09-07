using Airport.Models.Entities;
using Airport.Models.Enums;
using Airport.Models.Interfaces;
using Airport.Services.Creators;

namespace Airport.Services.Factories
{
    public class FlightCreatorFactory : IFlightCreatorFactory
    {
        public FlightCreatorFactory()
        {
        }

        public IFlightDTOFactory GetConcreteCreator(IFlight flight)
        {
            if (flight == null) throw new ArgumentNullException(nameof(flight));
            return flight.FlightType switch
            {
                FlightType.Landing => new LandingDTOFactory(),
                FlightType.Departure => new DepartureDTOFactory(),
                _ => throw new Exception(),
            };
        }
        public IFlightCreator GetConcreteCreator(Flight flight)
        {
            if (flight == null) throw new ArgumentNullException(nameof(flight));
            return flight switch
            {
                Landing => new LandingCreator(),
                Departure => new DepartureCreator(),
                _ => throw new Exception(),
            };
        }
    }
}
