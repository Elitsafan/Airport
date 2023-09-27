using Microsoft.AspNetCore.Mvc;

namespace Airport.Models.Interfaces
{
    public interface IAirportService
    {
        Task<IActionResult> GetStatus();
        Task<IActionResult> Start();
        Task<IActionResult> GetSummary();
        bool HasStarted { get; }
    }
}
