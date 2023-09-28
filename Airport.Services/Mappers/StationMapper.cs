using Airport.Models.DTOs;
using Airport.Models.Entities;
using Airport.Models.Interfaces;

namespace Airport.Services.Mappers
{
    public class StationMapper : IEntityMapper<Station, StationDTO>
    {
        public Station Map(StationDTO model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            return new Station
            {
                StationId = model.StationId,
                EstimatedWaitingTime = model.WaitingTime,
            };
        }

        public StationDTO Map(Station entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            return new StationDTO
            {
                StationId = entity.StationId,
                WaitingTime = entity.EstimatedWaitingTime,
            };
        }
        public void Dispose()
        {
        }
    }
}
