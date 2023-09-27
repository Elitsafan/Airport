using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Airport.Models.Entities
{
    [BsonDiscriminator]
    [BsonKnownTypes(typeof(Departure), typeof(Landing))]
    public abstract class Flight
    {
        private List<StationOccupationDetails>? _stationOccupationDetails;

        [BsonId]
        public ObjectId FlightId { get; set; }
        [BsonElement("route_id")]
        public ObjectId? RouteId { get; set; }

        [BsonElement("stations_occupation_details")]
        public List<StationOccupationDetails> StationOccupationDetails
        {
            get
            {
                _stationOccupationDetails ??= new List<StationOccupationDetails>();
                return _stationOccupationDetails;
            }
        }
    }
}