using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Airport.Models.Entities
{
    [Table("Routes")]
    public class Route
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RouteId { get; set; }
        [Required]
        public string RouteName { get; set; } = null!;
        public virtual ICollection<TrafficLight>? TrafficLights { get; set; } = new HashSet<TrafficLight>();
        public virtual ICollection<Direction>? Directions { get; set; } = new HashSet<Direction>();
        public virtual ICollection<Flight>? Flights { get; set; } = new HashSet<Flight>();
    }
}
