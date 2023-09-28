using Airport.Models.Entities;
using Airport.Models.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Threading;
using MongoDB.Bson;

namespace Airport.Services.Providers
{
    public class TrafficLightLogicProvider : ITrafficLightLogicProvider
    {
        #region Fields
        private readonly IServiceProvider _serviceProvider;
        private readonly ITrafficLightLogicFactory _trafficLightLogicFactory;
        private readonly HashSet<ITrafficLightLogic> _trafficLights;
        #endregion

        public TrafficLightLogicProvider(
            IServiceProvider serviceProvider,
            ITrafficLightLogicFactory trafficLightLogicFactory)
        {
            _serviceProvider = serviceProvider;
            _trafficLightLogicFactory = trafficLightLogicFactory;
            using var trafficLightRepository = serviceProvider
                .CreateAsyncScope()
                .ServiceProvider
                .GetRequiredService<ITrafficLightRepository>();
            // Creates the traffic light logics
            _trafficLights = new HashSet<ITrafficLightLogic>(
                new JoinableTaskFactory(new JoinableTaskContext())
                .Run(trafficLightRepository.GetAllAsync)
                .Select(trafficLight => _trafficLightLogicFactory.CreateTrafficLightLogic(trafficLight).Create()));
        }

        public async Task<IEnumerable<ITrafficLightLogic>> FindByRouteIdAsync(ObjectId routeId)
        {
            using var trafficLightRepository = _serviceProvider
                .CreateAsyncScope()
                .ServiceProvider
                .GetRequiredService<ITrafficLightRepository>();
            return (await trafficLightRepository
                .GetTrafficLightsByRouteIdAsync(routeId))
                .Select(GetITrafficLightLogic)
                .ToList();
        }
        private ITrafficLightLogic GetITrafficLightLogic(TrafficLight trafficLight) => trafficLight == null
            ? throw new ArgumentNullException(nameof(trafficLight))
            : _trafficLights.FirstOrDefault(s => s.StationId == trafficLight.StationId)
            ?? throw new ArgumentException("Traffic light not found", nameof(trafficLight));
    }
}
