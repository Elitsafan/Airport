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
        private readonly IStationRepository _stationRepository;
        private readonly IStationFlightRepository _stationFlightRepository;
        private readonly IFlightRepository _flightRepository;
        private readonly IEntityMapper<Flight, IFlight> _flightMapper;
        private readonly IEntityMapper<Station, StationDTO> _stationMapper;
        #endregion

        public AirportService(
            IStationRepository stationRepository,
            IFlightRepository flightRepository,
            IStationFlightRepository stationFlightRepository,
            IEntityMapper<Flight, IFlight> flightMapper,
            IEntityMapper<Station, StationDTO> stationMapper)
        {
            _stationRepository = stationRepository;
            _flightRepository = flightRepository;
            _stationFlightRepository = stationFlightRepository;
            _flightMapper = flightMapper;
            _stationMapper = stationMapper;
        }

        public bool HasStarted => _hasStarted;

        public IActionResult GetStatus()
        {
            List<IFlight> landings = null!;
            List<IFlight> departures = null!;
            List<StationDTO> stations = null!;
            landings = _flightRepository
                .OfType<Landing>()
                .Select(_flightMapper.Map)
                .ToList();
            departures = _flightRepository
                .OfType<Departure>()
                .Select(_flightMapper.Map)
                .ToList();
            stations = _stationRepository
                .GetAll()
                .Select(_stationMapper.Map)
                .ToList();
            return new JsonResult(null)
            {
                ContentType = "application/json",
                Value = new
                {
                    stations,
                    landings,
                    departures
                },
                StatusCode = StatusCodes.Status200OK
            };
        }

        public IActionResult GetSummary()
        {
            // Stations of each flight
            var stationsSummary = _stationFlightRepository
                .GetAll()
                .GroupBy(sf => sf.FlightId, (id, sfCollection) => new
                {
                    FlightId = id,
                    Stations = sfCollection
                    .OrderBy(s => s.Entrance)
                    .Select(f => new
                    {
                        f.StationId,
                        f.Entrance,
                        f.Exit
                    })
                })
                .ToList();
            // Flights
            var flights = _flightRepository
                .GetAll()
                .Select(f => new
                {
                    f.FlightId,
                    FlightType = f.ConvertToFlightType()
                })
                .ToList();
            // Joins by FlightId
            // Left = flights
            // Right = stations
            var flightsSummary = flights.Join(
                stationsSummary,
                l => l.FlightId,
                r => r.FlightId,
                (l, r) => new
                {
                    l.FlightId,
                    r.Stations,
                    l.FlightType
                })
                .ToList();

            // Sorts by entrance
            // Stations are already sorted by entrance
            flightsSummary.Sort(
                (left, right) => DateTime.Compare(
                    left.Stations.First().Entrance,
                    right.Stations.First().Entrance));

            return new JsonResult(null)
            {
                ContentType = "application/json",
                Value = flightsSummary,
                StatusCode = StatusCodes.Status200OK
            };
        }

        public IActionResult Start()
        {
            if (_hasStarted)
                return new JsonResult(null)
                {
                    ContentType = "application/json",
                    StatusCode = StatusCodes.Status200OK,
                    Value = "Already started"
                };
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
