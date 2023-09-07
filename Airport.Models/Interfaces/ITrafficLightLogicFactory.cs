using Airport.Models.Entities;

namespace Airport.Models.Interfaces
{
    public interface ITrafficLightLogicFactory
    {
        ITrafficLightLogicCreator CreateTrafficLightLogic(TrafficLight trafficLight);
    }
}