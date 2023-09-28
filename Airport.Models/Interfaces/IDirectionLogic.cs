using MongoDB.Bson;

namespace Airport.Models.Interfaces
{
    public interface IDirectionLogic
    {
        ObjectId From { get; }
        ObjectId To { get; }
    }
}
