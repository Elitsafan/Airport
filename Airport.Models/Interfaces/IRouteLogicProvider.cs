namespace Airport.Models.Interfaces
{
    public interface IRouteLogicProvider
    {
        IEnumerable<IRouteLogic> LandingRoutes { get; }
        IEnumerable<IRouteLogic> DepartureRoutes { get; }
    }
}
