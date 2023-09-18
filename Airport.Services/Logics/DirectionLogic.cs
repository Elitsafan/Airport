using Airport.Models.Entities;
using Airport.Models.Interfaces;

namespace Airport.Services.Logics
{
    public class DirectionLogic : IDirectionLogic
    {
        private readonly Direction _direction;
        public DirectionLogic(Direction direction) => _direction = direction;
        public int DirectionId => _direction.DirectionId;
        public int? From => _direction.From;
        public int? To => _direction.To;
        public int? RouteId => _direction.RouteId;
    }
}
