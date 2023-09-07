using Microsoft.AspNetCore.Mvc;

namespace Airport.Models.Interfaces
{
    public interface IAirportService
    {
        IActionResult GetStatus();
        IActionResult Start();
        IActionResult GetSummary();
        bool HasStarted { get; }
    }
}
