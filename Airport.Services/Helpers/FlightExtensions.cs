using Airport.Models.Entities;
using Airport.Models.Enums;

namespace Airport.Services.Helpers
{
    public static class FlightExtensions
    {
        public static FlightType? ConvertToFlightType(this Flight flight) => flight is null 
            ? null 
            : flight is Departure 
                ? FlightType.Departure
                : FlightType.Landing;
    }
}
