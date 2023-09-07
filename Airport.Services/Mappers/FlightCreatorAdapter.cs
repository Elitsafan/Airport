using Airport.Models.Enums;
using Airport.Models.Interfaces;
using Airport.Services.Factories;
using Airport.Services.Creators;

namespace Airport.Services.Mappers
{
    public class FlightCreatorAdapter : IEntityMapper<IFlightCreator, IFlightDTOFactory>
    {
        public Task<IFlightCreator> MapAsync(IFlightDTOFactory creator)
        {
            if (creator == null) throw new ArgumentNullException(nameof(creator));
            return creator.Create().FlightType switch
            {
                FlightType.Landing => Task.FromResult<IFlightCreator>(new LandingCreator()),
                FlightType.Departure => Task.FromResult<IFlightCreator>(new DepartureCreator()),
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
