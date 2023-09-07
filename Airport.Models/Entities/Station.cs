using System.ComponentModel.DataAnnotations.Schema;

namespace Airport.Models.Entities
{
    [Table("Stations")]
    public class Station
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StationId { get; set; }
        public DateTime? Entrance { get; set; }
        public DateTime? Exit { get; set; }
        public TimeSpan EstimatedWaitingTime { get; set; }
        public virtual TrafficLight? TrafficLight { get; set; }
        public virtual ICollection<Direction>? DirectionsFrom { get; set; } = new HashSet<Direction>();
        public virtual ICollection<Direction>? DirectionsTo { get; set; } = new HashSet<Direction>();
        public virtual ICollection<Flight>? Flights { get; set; } = new HashSet<Flight>();
    }
}