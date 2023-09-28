using Airport.Models.Entities;
using Airport.Models.Interfaces;
using MongoDB.Bson;

namespace Airport.Services.Logics
{
    public class DirectionLogic : IDirectionLogic
    {
        private readonly Direction _direction;
        public DirectionLogic(Direction direction) => _direction = direction;
        public ObjectId From => _direction.From;
        public ObjectId To => _direction.To;
    }
}
