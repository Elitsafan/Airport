using Airport.Models.DTOs;
using Airport.Models.Entities;
using Airport.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Airport.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        #region Fields
        private readonly IFlightService _flightSvc;
        private readonly IEntityMapper<Flight, IFlight> _flightMapper;
        private readonly ILogger<FlightsController> _logger;
        #endregion

        public FlightsController(
            IFlightService flightSvc,
            IEntityMapper<Flight, IFlight> flightMapper,
            ILogger<FlightsController> logger)
        {
            _logger = logger;
            _flightSvc = flightSvc;
            _flightMapper = flightMapper;
        }

        // GET: api/Flights/Landing
        [HttpPost("Landing")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Landing([FromBody] LandingDTO flight)
        {
            _flightSvc.ProcessFlight(_flightMapper.Map(flight));
            return CreatedAtRoute(nameof(AirportController.Status), StatusCodes.Status201Created);
        }

        // GET: api/Flights/Departure
        [HttpPost("Departure")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Departure([FromBody] DepartureDTO flight)
        {
            _flightSvc.ProcessFlight(_flightMapper.Map(flight));
            return CreatedAtRoute(nameof(AirportController.Status), StatusCodes.Status201Created);
        }
    }
}
