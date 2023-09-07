using System.ComponentModel.DataAnnotations.Schema;

namespace Airport.Models.Entities
{
    [Table("StationsFlights")]
    public class StationFlight
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid FlightId { get; set; }
        public int StationId { get; set; }
        public DateTime Entrance { get; set; }
        public DateTime Exit { get; set; }
    }
}
