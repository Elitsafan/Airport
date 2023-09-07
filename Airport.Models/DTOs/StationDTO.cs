using Newtonsoft.Json;

namespace Airport.Models.DTOs
{
    public class StationDTO
    {
        public int StationId { get; set; }
        [JsonIgnore]
        public DateTime? EntranceTime { get; set; }
        [JsonIgnore]
        public DateTime? ExitTime { get; set; }
        [JsonIgnore]
        public TimeSpan WaitingTime { get; set; }
    }
}
