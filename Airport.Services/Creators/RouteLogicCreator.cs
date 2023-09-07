using Airport.Models.Interfaces;
using Airport.Services.Logics;

namespace Airport.Services.Creators
{
    public class RouteLogicCreator : IRouteLogicCreator
    {
        #region Fields
        private readonly int _routeId;
        private readonly string _routeName;
        private readonly IServiceProvider _serviceProvider;
        #endregion

        public RouteLogicCreator(int routeId, string routeName, IServiceProvider serviceProvider)
        {
            _routeId = routeId;
            _routeName = routeName;
            _serviceProvider = serviceProvider;
        }

        public IRouteLogic Create() => new RouteLogic(_routeId, _routeName, _serviceProvider);
    }
}
