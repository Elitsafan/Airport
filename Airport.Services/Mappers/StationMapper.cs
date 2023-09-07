using Airport.Models.DTOs;
using Airport.Models.Entities;
using Airport.Models.Interfaces;

namespace Airport.Services.Mappers
{
    public class StationMapper : IEntityMapper<Station, StationDTO>
    {
        public Task<Station> MapAsync(StationDTO model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            return Task.FromResult(new Station
            {
                StationId = model.StationId,
                EstimatedWaitingTime = model.WaitingTime,
                Entrance = model.EntranceTime,
                Exit = model.ExitTime,
            });
        }
        public StationDTO Map(Station entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            return new StationDTO
            {
                WaitingTime = entity.EstimatedWaitingTime,
                EntranceTime = entity.Entrance,
                ExitTime = entity.Exit,
                StationId = entity.StationId,
            };
        }
        public void Dispose()
        {
        }
    }
}
