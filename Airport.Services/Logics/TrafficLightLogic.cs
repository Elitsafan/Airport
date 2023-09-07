using Airport.Models.Entities;
using Airport.Models.Enums;
using Airport.Models.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Airport.Services.Logics
{
    public class TrafficLightLogic : ITrafficLightLogic
    {
        #region Fields
        private readonly IStationLogicProvider _stationLogicProvider;
        private readonly TrafficLight _trafficLight;
        // Station ths associate with the traffic light
        private readonly IStationLogic _stationLogic;
        // Each route has one or many stations near the traffic light,
        private readonly Dictionary<int, IEnumerable<IStationLogic?>> _routeIdToStandingByStationsDic;
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
                .FindBy(s => s.StationId == _trafficLight.StationId)
                .Single();
            Func<IStationLogic, bool> predicate = stationLogic => trafficLight.Routes != null &&
                trafficLight.Routes
                    .SelectMany(r => r.Directions?.Where(d => d.To == _trafficLight.StationId) ?? Enumerable.Empty<Direction>())
                .Select(d => d.From!)
                .Contains(stationLogic.StationId);
            _stationsBeforeTrafficLight = _stationLogicProvider
                .FindBy(sl => predicate(sl));
            _isOccupied = station => station.CurrentFlightId.HasValue;
            var directionLogicProvider = serviceProvider.GetRequiredService<IDirectionLogicProvider>();
            // route id as the key, matched stations as the value
            _routeIdToStandingByStationsDic = new(trafficLight.Routes?
                .Select(r => new KeyValuePair<int, IEnumerable<IStationLogic?>>(
                    r.RouteId,
                    directionLogicProvider.GetStationsByTargetAndRoute(_trafficLight.StationId!.Value, r.RouteId))) ??
                    Enumerable.Empty<KeyValuePair<int, IEnumerable<IStationLogic?>>>());
        }

        #region Properties
        public int StationId => _stationLogic.StationId;
        public int TrafficLightId => _trafficLight.TrafficLightId;
        public IEnumerable<IStationLogic?> this[int routeId] => _routeIdToStandingByStationsDic[routeId];
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
