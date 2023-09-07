using System.ComponentModel.DataAnnotations.Schema;

namespace Airport.Models.Entities
{
    [Table("Directions")]
    public class Direction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DirectionId { get; set; }
        public int? RouteId { get; set; }
        public int? From { get; set; }
        public int? To { get; set; }
        public virtual Station? StationFrom { get; set; }
        public virtual Station? StationTo { get; set; }
        public virtual Route? Route { get; set; }
    }
}
