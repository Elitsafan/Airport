using Airport.Models.Interfaces;

namespace Airport.Models.EventArgs
{
    public class FlightRunDoneEventArgs : System.EventArgs
    {
        public FlightRunDoneEventArgs(IFlightLogic flight) => FlightDone = flight;
        public IFlightLogic FlightDone { get; }
    }
}