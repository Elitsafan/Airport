using Airport.Models.Entities;
using Airport.Models.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;

namespace Airport.Services.Providers
{
    public class StationLogicProvider : IStationLogicProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly HashSet<IStationLogic> _stations;

        public StationLogicProvider(IServiceProvider serviceProvider, IStationLogicFactory stationLogicFactory)
        {
            _serviceProvider = serviceProvider;
            using var stationRepository = serviceProvider
                .CreateAsyncScope()
                .ServiceProvider
                .GetRequiredService<IStationRepository>();
            // Creates the station logics
            _stations = new HashSet<IStationLogic>(stationRepository
                .GetAll()
                .Select(stationLogicFactory.CreateStationLogic));
        }

        public IQueryable<IStationLogic> FindBy(Expression<Func<IStationLogic, bool>> predicate) => _stations
            .AsQueryable()
            .Where(predicate);
        public IStationLogic? FindById(int id) => _stations.FirstOrDefault(s => s.StationId == id);
        public async Task<IEnumerable<IStationLogic>> FindByRouteId(int routeId)
        {
            using var routeRepository = _serviceProvider
                .CreateAsyncScope()
                .ServiceProvider
                .GetRequiredService<IRouteRepository>();
            var route = await routeRepository.GetByIdAsync(routeId);
            var stations = route.Directions?
                .Select(d => d.StationFrom)
                .Concat(
                    route.Directions!.Select(d => d.StationTo))
                .Distinct()
                ?? Enumerable.Empty<Station>();
            return stations.Any()
                ? _stations.Where(sl => stations.Any(s => s!.StationId == sl.StationId)).ToList()
                : Enumerable.Empty<IStationLogic>();
        }
        public IEnumerable<IStationLogic> GetAll() => _stations;
    }
}
