using Airport.Models.Entities;

namespace Airport.Models.Interfaces
{
    public interface IFlightLogicFactory
    {
        IFlightLogic Create(Flight flight);
    }
}
