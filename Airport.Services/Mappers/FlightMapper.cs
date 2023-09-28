using Airport.Models.Entities;
using Airport.Models.Interfaces;

namespace Airport.Services.Mappers
{
    public class FlightMapper : IEntityMapper<Flight, IFlight>
    {
        #region Fields
        private readonly IFlightCreatorFactory _flightCreatorFactory;
        private readonly IEntityMapper<IFlightCreator, IFlightDTOFactory> _flightCreatorMapper;
        #endregion

        public FlightMapper(
            IFlightCreatorFactory flightCreatorFactory,
            IEntityMapper<IFlightCreator, IFlightDTOFactory> creatorMapper)
        {
            _flightCreatorFactory = flightCreatorFactory;
            _flightCreatorMapper = creatorMapper;
        }

        public Flight Map(IFlight model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            var creator = _flightCreatorMapper.Map(_flightCreatorFactory.GetConcreteCreator(model));
            var createdFlight = creator.CreateFlight();
            createdFlight.FlightId = model.FlightId;
            return createdFlight;
        }
        public IFlight Map(Flight entity)
        {
            if (entity == null) 
                throw new ArgumentNullException(nameof(entity));
            var creator = _flightCreatorMapper.Map(_flightCreatorFactory.GetConcreteCreator(entity));
            var createdFlight = creator.Create();
            createdFlight.FlightId = entity.FlightId;
            return createdFlight;
        }
        public void Dispose()
        {
        }
    }
}
