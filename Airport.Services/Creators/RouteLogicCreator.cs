using Airport.Models.Interfaces;
using Airport.Services.Logics;
using MongoDB.Bson;

namespace Airport.Services.Creators
{
    public class RouteLogicCreator : IRouteLogicCreator
    {
        #region Fields
        private readonly ObjectId _routeId;
        private readonly string _routeName;
        private readonly IServiceProvider _serviceProvider;
        #endregion

        public RouteLogicCreator(ObjectId routeId, string routeName, IServiceProvider serviceProvider)
        {
            _routeId = routeId;
            _routeName = routeName;
            _serviceProvider = serviceProvider;
        }

        public IRouteLogic Create() => new RouteLogic(_routeId, _routeName, _serviceProvider);
    }
}
