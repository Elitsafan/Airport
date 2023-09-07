using Airport.Models.Entities;
using Airport.Models.Interfaces;
using Airport.Services.Logics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Airport.Services.Factories
{
    public class StationLogicFactory : IStationLogicFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public StationLogicFactory(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;
        public IStationLogic CreateStationLogic(Station station)
        {
            var logger = _serviceProvider.GetRequiredService<ILogger<StationLogic>>();
            return new StationLogic(_serviceProvider, logger, station);
        }
    }
}
