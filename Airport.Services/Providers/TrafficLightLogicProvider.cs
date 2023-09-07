using Airport.Models.Interfaces;
using Microsoft.Extensions.DependencyInjection;

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
            _trafficLights = new HashSet<ITrafficLightLogic>(trafficLightRepository
                .GetAll()
                .Select(trafficLight => _trafficLightLogicFactory.CreateTrafficLightLogic(trafficLight).Create()));
        }

        public IEnumerable<ITrafficLightLogic> FindByRouteId(int routeId)
        {
            using var trafficLightRepository = _serviceProvider
                .CreateAsyncScope()
                .ServiceProvider
                .GetRequiredService<ITrafficLightRepository>();
            var trafficLights = trafficLightRepository
                .GetAll()
                .AsEnumerable()
                .Where(t => t.Routes != null && t.Routes.Any(r => r.RouteId == routeId));
            return _trafficLights
                .Where(tl => trafficLights.Any(t => tl.TrafficLightId == t.TrafficLightId))
                .ToList();
        }
    }
}
