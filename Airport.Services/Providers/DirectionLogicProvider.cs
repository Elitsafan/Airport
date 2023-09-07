using Airport.Models.Entities;
using Airport.Models.Interfaces;
using Microsoft.Extensions.DependencyInjection;

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
            _directions = new HashSet<IDirectionLogic>(routeRepository
                .GetAll()
                .AsEnumerable()
                .SelectMany(r => r.Directions ?? Enumerable.Empty<Direction>())
                .Select(directionLogicFactory.CreateDirectionLogic)
                .ToList());
        }

        public async Task<IEnumerable<IDirectionLogic>> FindByRouteId(int routeId)
        {
            using var routeRepository = _serviceProvider
                .CreateAsyncScope()
                .ServiceProvider
                .GetRequiredService<IRouteRepository>();
            var route = await routeRepository.GetByIdAsync(routeId);
            return _directions
                .Where(dl => route.Directions != null && route.Directions.Any(d => d.DirectionId == dl.DirectionId))
                .ToList();
        }

        public IEnumerable<IStationLogic?> GetStationsByTargetAndRoute(int stationLogicId, int routeId)
        {
            var stationLogicProvider = _serviceProvider.GetRequiredService<IStationLogicProvider>();
            return _directions
                .Where(d => d.RouteId == routeId && d.To == stationLogicId && d.From.HasValue)
                .Select(d => stationLogicProvider.FindById(d.From!.Value))
                .ToList();
        }
    }
}
