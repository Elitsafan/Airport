﻿using Airport.Models.Entities;
using Airport.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Airport.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AirportController : ControllerBase
    {
        private readonly IAirportService _airportService;
        private readonly ILogger<AirportController> _logger;

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
        public IActionResult Status() => !_airportService.HasStarted
            ? StatusCode(StatusCodes.Status503ServiceUnavailable)
            : Ok(_airportService.GetStatus());

        // GET: api/Airport/Start
        [HttpGet("Start", Name = nameof(Start))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Start() => Ok(_airportService.Start());

        // GET: api/Airport/Summary
        [HttpGet("Summary", Name = nameof(Summary))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public IActionResult Summary() => !_airportService.HasStarted
            ? StatusCode(StatusCodes.Status503ServiceUnavailable)
            : Ok(_airportService.GetSummary());
    }
}
