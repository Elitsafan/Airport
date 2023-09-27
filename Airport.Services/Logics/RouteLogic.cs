﻿using Airport.Models.Enums;
using Airport.Models.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Threading;
using MongoDB.Bson;
using System.Collections;

namespace Airport.Services.Logics
{
    public class RouteLogic : IRouteLogic
    {
        #region Fields
        // Syncs all stations of the route 
        private readonly AsyncSemaphore _semTotalStations;
        private readonly FlightType _flightType;
        private readonly IServiceProvider _serviceProvider;
        private readonly IDirectionLogicProvider _directionLogicProvider;
        private readonly IStationLogicProvider _stationLogicProvider;
        private readonly ITrafficLightLogicProvider _trafficLightLogicProvider;
        private readonly ILogger<IRouteLogic> _logger;
        // Gets traffic logic by id
        private readonly Dictionary<ObjectId, ITrafficLightLogic> _trafficLightLogics;
        // Stations of the route
        private readonly List<IStationLogic> _stations;
        // Directions of the route
        private readonly List<IDirectionLogic> _directions;
        // Gets an AsyncSemaphore by station id
        // Used for multiple entrance attempts
        private readonly Dictionary<ObjectId, AsyncSemaphore> _stationLogicsSync;
        #endregion

        public RouteLogic(ObjectId routeId, string routeName, IServiceProvider serviceProvider)
        {
            if (string.IsNullOrWhiteSpace(routeName))
                throw new ArgumentException("Route name is null or empty or whitespace", nameof(routeName));
            RouteId = routeId;
            RouteName = routeName;
            Enum.TryParse(routeName, true, out _flightType);

            _serviceProvider = serviceProvider;
            _logger = _serviceProvider.GetRequiredService<ILogger<IRouteLogic>>();
            _directionLogicProvider = serviceProvider.GetRequiredService<IDirectionLogicProvider>();
            _stationLogicProvider = serviceProvider.GetRequiredService<IStationLogicProvider>();
            _trafficLightLogicProvider = serviceProvider.GetRequiredService<ITrafficLightLogicProvider>();
            // Sets the traffic lights of the route
            _trafficLightLogics = new(new JoinableTaskFactory(new JoinableTaskContext())
                .Run(() => _trafficLightLogicProvider.FindByRouteIdAsync(routeId))
                .Select(tll => new KeyValuePair<ObjectId, ITrafficLightLogic>(tll.StationId, tll))
                .ToList());
            // Sets the stations of the route
            _stations = new(new JoinableTaskFactory(new JoinableTaskContext())
                .Run(() => _stationLogicProvider.FindByRouteIdAsync(routeId)));
            // Sets the directions of the route
            _directions = new(new JoinableTaskFactory(
                new JoinableTaskContext())
                .Run(() => _directionLogicProvider.GetDirectionsByRouteIdAsync(routeId)));
            // holds the count of all routes exist
            int countRoutes;
            using (var routesRepository = _serviceProvider
                .CreateScope()
                .ServiceProvider
                .GetRequiredService<IRouteRepository>())
                countRoutes = new JoinableTaskFactory(new JoinableTaskContext())
                    .Run(routesRepository.GetAllAsync)
                    .Count();
            _stationLogicsSync = new();
            // Scans all the route legs and matches a semaphore to each traffic light
            ScanNextLeg(countRoutes, null);
            // Limits the number of flights that can start run 
            _semTotalStations = new AsyncSemaphore(_stations.Count);
        }

        #region Properties
        public ObjectId RouteId { get; }
        public string RouteName { get; }
        #endregion

        public async Task<AsyncSemaphore.Releaser> StartRun() => await _semTotalStations.EnterAsync();
        public async Task<AsyncSemaphore.Releaser> GetRightOfWay(
            IStationLogic? source,
            IStationLogic target,
            CancellationToken token = default)
        {
            // If target is a traffic light than waits to right of way
            // otherwise, moves on
            if (!IsTrafficLight(target))
                return default;
            // Gets the relevant semaphore
            _stationLogicsSync.TryGetValue(target.StationId, out var semaphore);
            // Creates a waiting array
            var waitingArray = GetNextStationsOf(target)
                .Select(s => s.AvailableWaitHandle)
                .ToArray();
            // If there is a traffic jam, waits
            if (waitingArray.Any() && IsDeadEnd(target))
            {
                target.AvailableWaitHandle.WaitOne();
                WaitHandle.WaitAny(waitingArray);
            }
            // If it is its right of way, moves on,
            // else waits for it
            if (!HasRightOfWay(source, target))
                return semaphore is null
                    ? default
                    : await semaphore.EnterAsync(token);
            return default;
        }
        public IEnumerable<IStationLogic> GetStartStations() => Start();
        public IEnumerable<IStationLogic> GetNextStationsOf(IStationLogic stationLogic)
        {
            if (stationLogic == null)
                throw new ArgumentNullException(nameof(stationLogic));
            if (!_stations.Contains(stationLogic))
                throw new ArgumentException("Station not found");
            return _stations
               .Where(s => _directions.Any(
                   d => d.From == stationLogic.StationId && d.To == s.StationId))
               .ToList();
        }
        public IEnumerator<IStationLogic> GetEnumerator() => _stations.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        //public void Dispose() => _semaphore?.Dispose();
        public bool IsTrafficLight(IStationLogic stationLogic) => _trafficLightLogics.ContainsKey(stationLogic.StationId);
        public bool HasRightOfWay(IStationLogic? source, IStationLogic target)
        {
            if (!_trafficLightLogics.TryGetValue(target.StationId, out var trafficLightLogic))
                throw new Exception("Traffic logic not found.");
            // When source is null,
            // cheks with other routes/stations and their flight type
            return source is null
                ? !trafficLightLogic.IsAnyOtherFlightStandingBy(_flightType)
                : !trafficLightLogic.IsAnyOtherFlightStandingBy(source);
        }

        // Gets the first stations of the route
        private List<IStationLogic> Start() => _stations
            .FindAll(s => _directions.Select(
                d => d.From).Except(_directions.Select(
                    d => d.To))
            .Contains(s.StationId));
        // Checks if entrance to 'target' will cause a dead end to the route
        // so that there will be no way out from it
        private bool IsDeadEnd(IStationLogic target)
        {
            if (target is null || !GetNextStationsOf(target).Any())
                return false;
            ITrafficLightLogic tll = _trafficLightLogics[target.StationId];
            var router = _serviceProvider.GetRequiredService<IRouteLogicProvider>();
            return router.AllRoutes
                .Where(r => r.RouteId != RouteId)
                .Where(r => r.Any(s => s.StationId == target.StationId))
                .All(rl => tll[rl.RouteId].All(s => rl.GetNextStationsOf(s).Count() < 2));
        }

        private void ScanNextLeg(int countRoutes, IStationLogic? station)
        {
            // Gets the next stations of 'station'
            var nextStations = station is null
                ? Start()
                : GetNextStationsOf(station);
            // End of route
            if (!nextStations.Any())
                return;
            StationLogicsSyncTryAdd(countRoutes, nextStations);
            // Keep scanning to end of route
            foreach (var s in nextStations)
                ScanNextLeg(countRoutes, s);
        }
        private void StationLogicsSyncTryAdd(int countRoutes, IEnumerable<IStationLogic> stations)
        {
            // Gets traffic lights only
            var trafficLights = stations
                .Where(IsTrafficLight)
                .ToList();
            // In case there less traffic lights than the total routes contains them, do nothing.
            if (trafficLights.Count < countRoutes)
                return;
            // Limits the number of traffic lights each route can have,
            // as the next stations.
            // For example, for 2 routes that contains the same 4 traffic lights,
            // and that 4 traffic lights are next stations of some 'x' station,
            // each semaphore will limit entrance to 2 traffic lights instead of 4.
            AsyncSemaphore semaphore = new(trafficLights.Count / countRoutes);
            trafficLights.ForEach(tl => _stationLogicsSync.TryAdd(
                tl.StationId,
                semaphore));
        }
    }
}