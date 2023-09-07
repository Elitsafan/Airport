using Airport.Models.Enums;
using Airport.Models.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Airport.Services.Providers
{
    public class RouteLogicProvider : IRouteLogicProvider
    {
        #region Fields
        private readonly ILogger<RouteLogicProvider> _logger;
        private readonly IAirportHubHandlerRegistrar _airportHubHandlerRegistrar;
        #endregion

        public RouteLogicProvider(
            IServiceProvider serviceProvider,
            IAirportHubHandlerRegistrar airportHubHandlerRegistrar,
            IRouteLogicFactory routeLogicFactory,
            ILogger<RouteLogicProvider> logger)
        {
            _logger = logger;
            _airportHubHandlerRegistrar = airportHubHandlerRegistrar;
            _airportHubHandlerRegistrar.Initialize();

            using var routeRepository = serviceProvider
                .CreateAsyncScope()
                .ServiceProvider
                .GetRequiredService<IRouteRepository>();
            // Creates route logics
            var routeLogics = routeRepository
                .GetAll()
                .Select(r => routeLogicFactory
                    .GetCreator(r.RouteId, r.RouteName)
                    .Create())
                .ToList();
            // Sets the route logics collections
            LandingRoutes = new List<IRouteLogic>(routeLogics
                .Where(rl => string.Compare(rl.RouteName, FlightType.Landing.ToString(), true) == 0));
            DepartureRoutes = new List<IRouteLogic>(routeLogics
                .Where(rl => string.Compare(rl.RouteName, FlightType.Departure.ToString(), true) == 0));
        }

        #region Properties
        public IEnumerable<IRouteLogic> LandingRoutes { get; private set; }
        public IEnumerable<IRouteLogic> DepartureRoutes { get; private set; }
        #endregion
    }
}
