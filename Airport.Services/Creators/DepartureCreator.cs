using Airport.Models.Entities;
using Airport.Models.Interfaces;

namespace Airport.Services.Creators
{
    internal class DepartureCreator : IFlightCreator
    {
        public Flight CreateFlight() => new Departure();
    }
}
