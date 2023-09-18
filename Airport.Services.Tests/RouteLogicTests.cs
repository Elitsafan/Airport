using Airport.Models.Entities;
using Airport.Models.Interfaces;
using Airport.Services.Logics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Airport.Services.Tests
{
    public class RouteLogicTests : IDisposable
    {
        #region Fields
        private ServiceProvider _serviceProvider;
        private string _routeName;
        private IRouteLogic _routeLogic;
        private ILogger<StationLogic> _slLogger;
        //private ILogger<DirectionLogic> _dlLogger;
        private IStationLogic[] _stationLogics;
        private IDirectionLogic[] _directionLogics;
        private Mock<IRouteRepository> _routeRepositoryMock;
        private Mock<IRouteLogicProvider> _routeLogicProviderMock;
        private Mock<IStationLogicProvider> _stationLogicProviderMock;
        private Mock<IDirectionLogicProvider> _directionLogicProviderMock;
        private Mock<ITrafficLightLogicProvider> _trafficLightLogicProviderMock;
        private int _routeId;
        #endregion

        public RouteLogicTests()
        {
            _routeRepositoryMock = new Mock<IRouteRepository>();
            _routeLogicProviderMock = new Mock<IRouteLogicProvider>();
            _stationLogicProviderMock = new Mock<IStationLogicProvider>();
            _directionLogicProviderMock = new Mock<IDirectionLogicProvider>();
            _trafficLightLogicProviderMock = new Mock<ITrafficLightLogicProvider>();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            serviceCollection.AddSingleton<IRouteLogicProvider>(factory => _routeLogicProviderMock.Object);
            serviceCollection.AddSingleton<IStationLogicProvider>(factory => _stationLogicProviderMock.Object);
            serviceCollection.AddSingleton<IDirectionLogicProvider>(factory => _directionLogicProviderMock.Object);
            serviceCollection.AddSingleton<ITrafficLightLogicProvider>(factory => _trafficLightLogicProviderMock.Object);
            serviceCollection.AddScoped<IRouteRepository>(factory => _routeRepositoryMock.Object);
            //serviceCollection.AddScoped<IFlightLogic>((logic) => _flightLogicMock.Object);
            _serviceProvider = serviceCollection.BuildServiceProvider();

            _routeId = 2;
            _routeName = "Departure";
            _slLogger = _serviceProvider.GetRequiredService<ILogger<StationLogic>>();
            //_dlLogger = _serviceProvider.GetRequiredService<ILogger<DirectionLogic>>();
            _stationLogics = new IStationLogic[]
            {
                new StationLogic(_serviceProvider, _slLogger, new Station { StationId = 1 }),
                new StationLogic(_serviceProvider, _slLogger, new Station { StationId = 2 }),
                new StationLogic(_serviceProvider, _slLogger, new Station { StationId = 3 })
            };
            _directionLogics = new IDirectionLogic[]
            {
                new DirectionLogic(new Direction
                {
                    DirectionId = 1,
                    From = 1,
                    To = 2,
                    RouteId = _routeId
                }),
                new DirectionLogic(new Direction
                {
                    DirectionId = 2,
                    From = 2,
                    To = 3,
                    RouteId = _routeId
                })
            };
            _routeRepositoryMock
                .Setup(x => x.GetAll())
                .Returns(() => new Route[]
                {
                    new Route { RouteId = _routeId },
                }.AsQueryable());
            _stationLogicProviderMock
                .Setup(x => x.FindByRouteId(_routeId))
                .ReturnsAsync(() => _stationLogics);
            _directionLogicProviderMock
                .Setup(x => x.FindByRouteId(_routeId))
                .ReturnsAsync(() => _directionLogics);
            _routeLogic = new RouteLogic(_routeId, _routeName, _serviceProvider);
            _routeLogicProviderMock
                .Setup(x => x.DepartureRoutes)
                .Returns(() => new IRouteLogic[]
                {
                    _routeLogic
                }.AsEnumerable());
        }

        [Fact]
        public void RouteId_WhenCalled_ReturnsSetValue_Test() => Assert.True(_routeLogic.RouteId == _routeId);
        [Fact]
        public void RouteName_WhenCalled_ReturnsSetValue_Test() => Assert.True(_routeLogic.RouteName == _routeName);
        [Fact]
        public void GetStartStations_WhenCalled_ReturnsFirstStation_Test() =>
            Assert.Equal(new IStationLogic[] { _stationLogics[0] }, _routeLogic.GetStartStations().ToList());
        [Fact]
        public void GetNextStationsOf_WhenCalledWithNullParam_ThrowsArgumentNullException_Test() => 
            Assert.Throws<ArgumentNullException>(() => _routeLogic.GetNextStationsOf(null!));
        [Fact]
        public void GetNextStationsOf_WhenCalledWithUnknownStation_ThrowsArgumentException_Test()
        {
            var unknown = new StationLogic(_serviceProvider, _slLogger, new Station());
            Assert.Throws<ArgumentException>(() => _routeLogic.GetNextStationsOf(unknown));
        }
        [Fact]
        public void GetNextStationsOf_WhenCalled_ReturnsNextStations_Test() =>
            Assert.Equal(new IStationLogic[] { _stationLogics[1] }, _routeLogic.GetNextStationsOf(_stationLogics[0]));

        public void Dispose() => _serviceProvider.Dispose();
    }
}
