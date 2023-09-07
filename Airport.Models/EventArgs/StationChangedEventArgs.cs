using Airport.Models.Entities;

namespace Airport.Models.EventArgs
{
    public class StationChangedEventArgs : System.EventArgs
    {
        public StationChangedEventArgs(Flight? flight) => Flight = flight;
        public Flight? Flight { get; }
    }
}