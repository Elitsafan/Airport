using System.ComponentModel.DataAnnotations.Schema;

namespace Airport.Models.Entities
{
    public abstract class Flight
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid FlightId { get; set; }
        public int? RouteId { get; set; }
        public virtual Route? Route { get; set; }
        public virtual ICollection<Station>? Stations { get; set; } = new HashSet<Station>();
    }
}