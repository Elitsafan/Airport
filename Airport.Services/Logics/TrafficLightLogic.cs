using Airport.Models.Entities;
using Airport.Models.Enums;
using Airport.Models.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Threading;
using MongoDB.Bson;
using System.Linq.Expressions;

namespace Airport.Services.Logics
{
    public class TrafficLightLogic : ITrafficLightLogic
    {
        #region Fields
        private readonly IStationLogicProvider _stationLogicProvider;
        private readonly TrafficLight _trafficLight;
        private readonly Route[] _routesContainingTrafficLight;
        // Station ths associate with the traffic light
        private readonly IStationLogic _stationLogic;
        // Each route has one or many stations near the traffic light,
        private readonly Dictionary<ObjectId, IEnumerable<IStationLogic>> _routeIdToStandingByStationsDic;
        // Gets the stations before the traffic light with their actual state
        private readonly IQueryable<IStationLogic> _stationsBeforeTrafficLight;
        // Station occupation predicate
        private readonly Func<IStationLogic, bool> _isOccupied;
        #endregion

        public TrafficLightLogic(IServiceProvider serviceProvider, TrafficLight trafficLight)
        {
            _stationLogicProvider = serviceProvider.GetRequiredService<IStationLogicProvider>();
            _trafficLight = trafficLight;
            _stationLogic = _stationLogicProvider
                .GetAll()
                .Single(s => s.StationId == _trafficLight.StationId);
            using var routeRepository = serviceProvider
                .CreateAsyncScope()
                .ServiceProvider
                .GetRequiredService<IRouteRepository>();
            _routesContainingTrafficLight = new JoinableTaskFactory(new JoinableTaskContext())
                .Run(() => routeRepository.GetRoutesByStationIdAsync(StationId))
                .ToArray();
            // Checks if a station comes before the traffic light
            Expression<Func<IStationLogic, bool>> isBeforeTrafficLight = stationLogic => _routesContainingTrafficLight
                .SelectMany(r => r.Directions.Where(d => d.To == _trafficLight.StationId))
                .Select(d => d.From)
                .Contains(stationLogic.StationId);
            _stationsBeforeTrafficLight = _stationLogicProvider
                .FindBy(isBeforeTrafficLight)
                .AsQueryable();
            _isOccupied = station => station.CurrentFlightId.HasValue;
            var directionLogicProvider = serviceProvider.GetRequiredService<IDirectionLogicProvider>();
            // route id as the key, matched stations as the value
            _routeIdToStandingByStationsDic = new(_routesContainingTrafficLight
                .Select(r => new KeyValuePair<ObjectId, IEnumerable<IStationLogic>>(
                    r.RouteId,
                    new JoinableTaskFactory(new JoinableTaskContext())
                    .Run(() => _stationLogicProvider.GetStationsByTargetAndRouteAsync(_trafficLight.StationId, r.RouteId)))));
        }

        #region Properties
        public ObjectId StationId => _stationLogic.StationId;
        public ObjectId TrafficLightId => _trafficLight.TrafficLightId;
        public IEnumerable<IStationLogic> this[ObjectId routeId] => _routeIdToStandingByStationsDic[routeId];
        #endregion

        public bool IsAnyOtherFlightStandingBy(FlightType flightType) => _stationsBeforeTrafficLight
            .Any(s => s.CurrentFlightType != flightType && _isOccupied(s));
        public bool IsAnyOtherFlightStandingBy(IStationLogic stationLogic) =>
            _stationsBeforeTrafficLight.Any(s
                => s.StationId != stationLogic.StationId &&
                _isOccupied(s) &&
                s.CurrentFlightType != stationLogic.CurrentFlightType);
    }
}
