using Airport.Models.Entities;
using Airport.Models.Interfaces;
using Airport.Services.Logics;

namespace Airport.Services.Factories
{
    public class DirectionLogicFactory : IDirectionLogicFactory
    {
        public IDirectionLogic CreateDirectionLogic(Direction direction) => new DirectionLogic(direction);
    }
}
