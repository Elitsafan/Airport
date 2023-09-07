using Airport.Models.Interfaces;
using Airport.Services.Creators;

namespace Airport.Services.Factories
{
    public class RouteLogicFactory : IRouteLogicFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public RouteLogicFactory(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public IRouteLogicCreator GetCreator(int routeId, string routeName) => new RouteLogicCreator(
            routeId,
            routeName,
            _serviceProvider);
    }
}
