using Airport.Models.Entities;
using Airport.Models.Interfaces;
using Airport.Services.Logics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Airport.Services.Factories
{
    public class FlightLogicFactory : IFlightLogicFactory
    {
        #region Fields
        private readonly IServiceProvider _serviceProvider;
        private readonly IRouteLogicProvider _router;
        private readonly ILogger<IFlightLogic> _logger;
        #endregion

        public FlightLogicFactory(
            IServiceProvider serviceProvider,
            IRouteLogicProvider router,
            ILogger<IFlightLogic> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _router = router;
        }
        public IFlightLogic Create(Flight flight)
        {
            var repository = _serviceProvider
                .CreateAsyncScope()
                .ServiceProvider
                .GetRequiredService<IFlightRepository>();

            return flight switch
            {
                Departure => new FlightLogic(repository, _router.DepartureRoutes.First(), _logger, flight),
                Landing => new FlightLogic(repository, _router.LandingRoutes.First(), _logger, flight),
                _ => throw new ArgumentException()
            };
        }
    }
}
