using Airport.Models.Entities;

namespace Airport.Models.Interfaces
{
    public interface IStationLogicFactory
    {
        IStationLogic CreateStationLogic(Station station);
    }
}
