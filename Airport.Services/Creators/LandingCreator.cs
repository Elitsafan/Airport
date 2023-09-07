using Airport.Models.Entities;
using Airport.Models.Interfaces;

namespace Airport.Services.Creators
{
    internal class LandingCreator : IFlightCreator
    {
        public Flight CreateFlight() => new Landing();
    }
}
