using Airport.Models.Enums;
using Airport.Models.Interfaces;
using Airport.Services.Creators;
using Airport.Services.Factories;

namespace Airport.Services.Mappers
{
    public class FlightCreatorAdapter : IEntityMapper<IFlightCreator, IFlightDTOFactory>
    {
        public IFlightCreator Map(IFlightDTOFactory creator)
        {
            if (creator == null) throw new ArgumentNullException(nameof(creator));
            return creator.Create().FlightType switch
            {
                FlightType.Landing => new LandingCreator(),
                FlightType.Departure => new DepartureCreator(),
                _ => throw new Exception(),
            };
        }
        public IFlightDTOFactory Map(IFlightCreator creator)
        {
            if (creator == null) throw new ArgumentNullException(nameof(creator));
            return creator switch
            {
                LandingCreator => new LandingDTOFactory(),
                DepartureCreator => new DepartureDTOFactory(),
                _ => throw new Exception(),
            };
        }
        public void Dispose()
        {
        }
    }
}
