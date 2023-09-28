using Airport.Models.Entities;
using Airport.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Airport.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AirportController : ControllerBase
    {
        #region Fields
        private readonly IAirportService _airportService;
        private readonly ILogger<AirportController> _logger; 
        #endregion

        public AirportController(
            IAirportService airportService,
            ILogger<AirportController> logger)
        {
            _airportService = airportService;
            _logger = logger;
        }

        // GET: api/Airport/Status
        [HttpGet("Status", Name = nameof(Status))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> Status() => !_airportService.HasStarted
            ? StatusCode(StatusCodes.Status503ServiceUnavailable)
            : Ok(await _airportService.GetStatus());

        // GET: api/Airport/Start
        [HttpGet("Start", Name = nameof(Start))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Start() => Ok(await _airportService.Start());

        // GET: api/Airport/Summary
        [HttpGet("Summary", Name = nameof(Summary))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> Summary() => !_airportService.HasStarted
            ? StatusCode(StatusCodes.Status503ServiceUnavailable)
            : Ok(await _airportService.GetSummary());
    }
}
