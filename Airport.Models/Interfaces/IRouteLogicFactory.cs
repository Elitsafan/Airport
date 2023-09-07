namespace Airport.Models.Interfaces
{
    public interface IRouteLogicFactory
    {
        IRouteLogicCreator GetCreator(int routeId, string routeName);
    }
}
