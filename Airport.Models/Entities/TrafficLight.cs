using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Airport.Models.Entities
{
    [Table("TrafficLights")]
    public class TrafficLight
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TrafficLightId { get; set; }
        [Required]
        public int? StationId { get; set; }
        public virtual Station? Station { get; set; }
        public virtual ICollection<Route>? Routes { get; set; } = new HashSet<Route>();
    }
}
