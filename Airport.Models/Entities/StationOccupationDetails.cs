using Airport.Models.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Airport.Models.Entities
{
    public class StationOccupationDetails : IStationOccupationDetails
    {
        [BsonElement("station_id")]
        public ObjectId StationId { get; set; }
        [BsonElement("entrance")]
        public DateTime Entrance { get; set; }
        [BsonElement("exit")]
        public DateTime Exit { get; set; }
    }
}
