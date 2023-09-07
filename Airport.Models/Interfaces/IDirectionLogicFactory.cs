using Airport.Models.Entities;

namespace Airport.Models.Interfaces
{
    public interface IDirectionLogicFactory
    {
        IDirectionLogic CreateDirectionLogic(Direction direction);
    }
}
