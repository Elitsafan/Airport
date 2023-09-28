using Airport.Models.Entities;
using MongoDB.Bson;

namespace Airport.Models.DTOs
{
    public class RouteDTO
    {
        private List<Direction>? _directions;

        public ObjectId RouteId { get; set; }
        public string RouteName { get; set; } = string.Empty;
        public List<Direction> Directions
        {
            get => _directions ?? new List<Direction>();
            set => _directions = value;
        }
    }
}
