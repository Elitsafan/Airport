using Airport.Models.Entities;
using Airport.Models.EventArgs;
using Microsoft.VisualStudio.Threading;
using System.Diagnostics.CodeAnalysis;

namespace Airport.Models.Interfaces
{
    public interface IFlightLogic : IDisposable
    {
        /// <summary>
        /// Occures when the <see cref="Flight"/> is done running
        /// </summary>
        event AsyncEventHandler<FlightRunDoneEventArgs>? FlightRunDone;
        /// <summary>
        /// The <see cref="Entities.Flight"/> that moves across stations
        /// </summary>
        Flight Flight { get; }
        /// <summary>
        /// The current <see cref="IStationLogic"/> that holds the <see cref="Flight"/>
        /// </summary>
        IStationLogic CurrentStation { get; }
        int RouteId { get; }
        [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "<Pending>")]
        Task ThrowIfCancellationRequested(CancellationTokenSource? source);
        /// <summary>
        /// Runs the current instance
        /// </summary>
        Task RunAsync();
        /// <summary>
        /// Exits from the <see cref="CurrentStation"/>
        /// </summary>
        /// <returns></returns>
        Task ExitCurrentStationAsync();
    }
}
