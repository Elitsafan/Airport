using Airport.Models.Entities;
using Airport.Models.Interfaces;
using Airport.Services.Logics;

namespace Airport.Services.Creators
{
    public class TrafficLightLogicCreator : ITrafficLightLogicCreator
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TrafficLight _trafficLight;

        public TrafficLightLogicCreator(IServiceProvider serviceProvider, TrafficLight trafficLight)
        {
            _serviceProvider = serviceProvider;
            _trafficLight = trafficLight;
        }

        public ITrafficLightLogic Create() => new TrafficLightLogic(_serviceProvider, _trafficLight);
    }
}
