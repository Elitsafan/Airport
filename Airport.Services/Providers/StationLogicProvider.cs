using Airport.Models.Entities;
using Airport.Models.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Threading;
using MongoDB.Bson;
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
            _stations = new HashSet<IStationLogic>(
                new JoinableTaskFactory(new JoinableTaskContext())
                .Run(stationRepository.GetAllAsync)
                .Select(stationLogicFactory.CreateStationLogic));
        }

        public IEnumerable<IStationLogic> FindBy(Expression<Func<IStationLogic, bool>> predicate) => GetAll()
            .AsQueryable()
            .Where(predicate);
        public async Task<IEnumerable<IStationLogic>> FindByRouteIdAsync(ObjectId routeId)
        {
            using var stationRepository = _serviceProvider
                .CreateAsyncScope()
                .ServiceProvider
                .GetRequiredService<IStationRepository>();
            return (await stationRepository
                .GetStationsByRouteIdAsync(routeId))
                .Select(GetIStationLogic)
                .ToList();
        }
        public async Task<IEnumerable<IStationLogic>> GetStationsByTargetAndRouteAsync(ObjectId stationLogicId, ObjectId routeId)
        {
            using var routeRepository = _serviceProvider
                .CreateAsyncScope()
                .ServiceProvider
                .GetRequiredService<IRouteRepository>();
            return (await routeRepository
                .GetRouteByIdAsync(routeId))
                .Directions
                .Where(d => d.To == stationLogicId)
                .Select(d => GetIStationLogic(d.To));
        }
        public IEnumerable<IStationLogic> GetAll() => _stations;

        private IStationLogic GetIStationLogic(Station station) => station == null
            ? throw new ArgumentNullException(nameof(station))
            : _stations.FirstOrDefault(s => s.StationId == station.StationId)
            ?? throw new ArgumentException("Station not found", nameof(station));
        private IStationLogic GetIStationLogic(ObjectId stationId) =>
            _stations.FirstOrDefault(s => s.StationId == stationId)
            ?? throw new ArgumentException("Station not found", nameof(stationId));
    }
}
