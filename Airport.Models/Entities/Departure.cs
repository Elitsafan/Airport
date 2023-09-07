using Airport.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Airport.Models.Entities
{
    [Table("Departures")]
    public class Departure : Flight
    {
    }
}
