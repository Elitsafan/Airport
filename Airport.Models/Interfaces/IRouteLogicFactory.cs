using MongoDB.Bson;

namespace Airport.Models.Interfaces
{
    public interface IRouteLogicFactory
    {
        IRouteLogicCreator GetCreator(ObjectId routeId, string routeName);
    }
}
