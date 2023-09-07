using Airport.Models.Entities;

namespace Airport.Models.Interfaces
{
    public interface IFlightCreatorFactory
    {
        IFlightDTOFactory GetConcreteCreator(IFlight flight);
        IFlightCreator GetConcreteCreator(Flight flight);
    }
}
