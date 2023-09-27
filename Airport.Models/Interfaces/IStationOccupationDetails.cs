using MongoDB.Bson;

namespace Airport.Models.Interfaces
{
    public interface IStationOccupationDetails
    {
        ObjectId StationId { get; set; }
        DateTime Entrance { get; set; }
        DateTime Exit { get; set; }
    }
}