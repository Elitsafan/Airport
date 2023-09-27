using Airport.Models.Entities;
using Airport.Models.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Threading;
using MongoDB.Bson;

namespace Airport.Services.Providers
{
    public class DirectionLogicProvider : IDirectionLogicProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly HashSet<IDirectionLogic> _directions;

        public DirectionLogicProvider(IServiceProvider serviceProvider, IDirectionLogicFactory directionLogicFactory)
        {
            _serviceProvider = serviceProvider;
            using var routeRepository = serviceProvider
                .CreateAsyncScope()
                .ServiceProvider
                .GetRequiredService<IRouteRepository>();
            // Creates the direction logics
            _directions = new HashSet<IDirectionLogic>(
                new JoinableTaskFactory(new JoinableTaskContext())
                .Run(routeRepository.GetAllAsync)
                .SelectMany(r => r.Directions)
                .Select(directionLogicFactory.CreateDirectionLogic)
                .ToList());
        }

        public async Task<IEnumerable<IDirectionLogic>> GetDirectionsByRouteIdAsync(ObjectId routeId)
        {
            using var routeRepository = _serviceProvider
                .CreateAsyncScope()
                .ServiceProvider
                .GetRequiredService<IRouteRepository>();
            return (await routeRepository
                .GetRouteByIdAsync(routeId))
                .Directions
                .Select(GetIDirectionLogic);
        }

        private IDirectionLogic GetIDirectionLogic(Direction direction) => direction == null
            ? throw new ArgumentNullException(nameof(direction))
            : _directions.FirstOrDefault(d => d.From == direction.From && d.To == direction.To)
            ?? throw new ArgumentException("Direction not found", nameof(direction));
    }
}
