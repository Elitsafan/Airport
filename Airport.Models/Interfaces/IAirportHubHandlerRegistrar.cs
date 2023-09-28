using Airport.Models.EventArgs;

namespace Airport.Models.Interfaces
{
    public interface IAirportHubHandlerRegistrar
    {
        void Initialize();
        Task OnFlightRunDone(object? sender, FlightRunDoneEventArgs e);
    }
}