using Airport.Models.Entities;
using Airport.Models.Interfaces;
using Airport.Services.Creators;

namespace Airport.Services.Factories
{
    public class TrafficLightLogicFactory : ITrafficLightLogicFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public TrafficLightLogicFactory(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public ITrafficLightLogicCreator CreateTrafficLightLogic(TrafficLight trafficLight) =>
            new TrafficLightLogicCreator(_serviceProvider, trafficLight);
    }
}
