using Airport.Models.Interfaces;

namespace Airport.Models.Helpers
{
    public class FlightTimeoutConfiguration : IFlightTimeoutConfiguration
    {
        public double Timeout { get; set; }
    }
}