using Airport.Models.DTOs;
using Airport.Models.Entities;
using Airport.Models.Interfaces;
using Airport.Services.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Airport.Services
{
    public class AirportService : IAirportService
    {
        #region Fields
        private static bool _hasStarted;
        private readonly IAirportDbContextSetup _dbSetup;
        private readonly IStationRepository _stationRepository;
        private readonly IFlightRepository _flightRepository;
        private readonly IRouteRepository _routeRepository;
        private readonly IEntityMapper<Flight, IFlight> _flightMapper;
        private readonly IEntityMapper<Station, StationDTO> _stationMapper;
        private readonly IEntityMapper<Route, RouteDTO> _routeMapper;
        #endregion

        public AirportService(
            IAirportDbContextSetup dbSetup,
            IStationRepository stationRepository,
            IFlightRepository flightRepository,
            IRouteRepository routeRepository,
            IEntityMapper<Flight, IFlight> flightMapper,
            IEntityMapper<Station, StationDTO> stationMapper,
            IEntityMapper<Route, RouteDTO> routeMapper)
        {
            _dbSetup = dbSetup;
            _stationRepository = stationRepository;
            _flightRepository = flightRepository;
            _routeRepository = routeRepository;
            _flightMapper = flightMapper;
            _stationMapper = stationMapper;
            _routeMapper = routeMapper;
        }

        public bool HasStarted => _hasStarted;

        public async Task<IActionResult> GetStatus()
        {
            List<StationDTO> stations = (await _stationRepository
                .GetAllAsync())
                .Select(_stationMapper.Map)
                .ToList();
            List<IFlight> landings = (await _flightRepository
                .OfTypeAsync<Landing>())
                .Select(_flightMapper.Map)
                .ToList();
            List<IFlight> departures = (await _flightRepository
                .OfTypeAsync<Departure>())
                .Select(_flightMapper.Map)
                .ToList();
            List<RouteDTO> routes = (await _routeRepository
                .GetAllAsync())
                .Select(_routeMapper.Map)
                .ToList();

            return new JsonResult(null)
            {
                ContentType = "application/json",
                Value = new
                {
                    stations,
                    landings,
                    departures,
                    routes
                },
                StatusCode = StatusCodes.Status200OK
            };
        }

        public async Task<IActionResult> GetSummary()
        {
            // Each instance has:
            //      Flight id
            //      Stations details
            //      Flight type
            var flightsSummary = (await _flightRepository
                .GetAllAsync())
                .Select(f => new
                {
                    f.FlightId,
                    Stations = f.StationOccupationDetails
                        .OrderBy(s => s.Entrance)
                        .ToList(),
                    FlightType = f.ConvertToFlightType()
                })
                .ToList();

            // Sorts by entrance
            // Stations are already sorted by entrance
            flightsSummary.Sort(
                (left, right) => !left.Stations.Any()
                    ? 1
                    : !right.Stations.Any()
                        ? 0
                        : DateTime.Compare(left.Stations.First().Entrance,
                        right.Stations.First().Entrance));

            return new JsonResult(null)
            {
                ContentType = "application/json",
                Value = flightsSummary,
                StatusCode = StatusCodes.Status200OK
            };
        }

        public async Task<IActionResult> Start()
        {
            if (_hasStarted)
                return new JsonResult(null)
                {
                    ContentType = "application/json",
                    StatusCode = StatusCodes.Status200OK,
                    Value = "Already started"
                };
            try
            {
                await _dbSetup.SeedDatabaseAsync();
            }
            catch (Exception)
            {

                throw;
            }
            _hasStarted = true;
            return new JsonResult(null)
            {
                ContentType = "application/json",
                StatusCode = StatusCodes.Status200OK,
                Value = "Started"
            };
        }
    }
}
