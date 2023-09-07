using System.ComponentModel.DataAnnotations.Schema;

namespace Airport.Models.Entities
{
    [Table("Landings")]
    public class Landing : Flight
    {
    }
}
