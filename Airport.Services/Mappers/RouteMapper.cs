using Airport.Models.DTOs;
using Airport.Models.Entities;
using Airport.Models.Interfaces;

namespace Airport.Services.Mappers
{
    public class RouteMapper : IEntityMapper<Route, RouteDTO>
    {
        public Route Map(RouteDTO model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            return new Route
            {
                RouteId = model.RouteId,
                RouteName = model.RouteName,
                Directions = model.Directions,
            };
        }

        public RouteDTO Map(Route entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            return new RouteDTO
            {
                RouteId = entity.RouteId,
                RouteName = entity.RouteName,
                Directions = entity.Directions,
            };
        }
        public void Dispose()
        {
        }
    }
}
