namespace Airport.Services.Tests
{
    public class RouteLogicTests : IDisposable
    {
        #region Fields
        private ServiceProvider _serviceProvider;
        private string _routeName;
        private IRouteLogic _routeLogic;
        private ILogger<StationLogic> _slLogger;
        private Mock<IDirectionLogic>[] _directionLogicMocks;
        private Mock<IStationLogic>[] _stationLogicMocks;
        private Mock<IRouteRepository> _routeRepositoryMock;
        private Mock<IRouteLogicProvider> _routeLogicProviderMock;
        private Mock<IStationLogicProvider> _stationLogicProviderMock;
        private Mock<IDirectionLogicProvider> _directionLogicProviderMock;
        private Mock<ITrafficLightLogicProvider> _trafficLightLogicProviderMock;
        private ObjectId[] _ids;
        private ObjectId _routeId;
        #endregion

        public RouteLogicTests()
        {
            _routeRepositoryMock = new Mock<IRouteRepository>();
            _routeLogicProviderMock = new Mock<IRouteLogicProvider>();
            _stationLogicProviderMock = new Mock<IStationLogicProvider>();
            _directionLogicProviderMock = new Mock<IDirectionLogicProvider>();
            _trafficLightLogicProviderMock = new Mock<ITrafficLightLogicProvider>();
            _ids = new ObjectId[]
            {
                ObjectId.Parse("000000000000000000000001"),
                ObjectId.Parse("000000000000000000000002"),
                ObjectId.Parse("000000000000000000000003")
            };
            _routeId = new ObjectId("650abb1ee574435a814d7ec1");
            _routeName = "Departure";

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<ILogger<StationLogic>>(factory => new Mock<ILogger<StationLogic>>().Object);
            serviceCollection.AddSingleton<ILogger<IRouteLogic>>(factory => new Mock<ILogger<IRouteLogic>>().Object);
            serviceCollection.AddSingleton<IRouteLogicProvider>(factory => _routeLogicProviderMock.Object);
            serviceCollection.AddSingleton<IStationLogicProvider>(factory => _stationLogicProviderMock.Object);
            serviceCollection.AddSingleton<IDirectionLogicProvider>(factory => _directionLogicProviderMock.Object);
            serviceCollection.AddSingleton<ITrafficLightLogicProvider>(factory => _trafficLightLogicProviderMock.Object);
            serviceCollection.AddScoped<IRouteRepository>(factory => _routeRepositoryMock.Object);
            _serviceProvider = serviceCollection.BuildServiceProvider();

            _slLogger = _serviceProvider.GetRequiredService<ILogger<StationLogic>>();
            // Stations
            _stationLogicMocks = new Mock<IStationLogic>[]
            {
                new Mock<IStationLogic>(),
                new Mock<IStationLogic>(),
                new Mock<IStationLogic>(),
            };
            _stationLogicMocks[0]
                .SetupGet(x => x.StationId)
                .Returns(_ids[0]);
            _stationLogicMocks[1]
                .SetupGet(x => x.StationId)
                .Returns(_ids[1]);
            _stationLogicMocks[2]
                .SetupGet(x => x.StationId)
                .Returns(_ids[2]);
            _stationLogicProviderMock
                .Setup(x => x.GetAll())
                .Returns(() => _stationLogicMocks.Select(sl => sl.Object));
            // Directions
            _directionLogicMocks = new Mock<IDirectionLogic>[]
            {
                new Mock<IDirectionLogic>(),
                new Mock<IDirectionLogic>()
            };
            _directionLogicMocks[0]
                .SetupGet(x => x.From)
                .Returns(_ids[0]);
            _directionLogicMocks[0]
                .SetupGet(x => x.To)
                .Returns(_ids[1]);
            _directionLogicMocks[1]
                .SetupGet(x => x.From)
                .Returns(_ids[1]);
            _directionLogicMocks[1]
                .SetupGet(x => x.To)
                .Returns(_ids[2]);
            // RouteRepository
            _routeRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(() => new Route[]
                {
                    new Route { RouteId = _routeId },
                });
            // Providers
            _stationLogicProviderMock
                .Setup(x => x.FindByRouteIdAsync(_routeId))
                .ReturnsAsync(() => _stationLogicMocks.Select(sl => sl.Object));
            _directionLogicProviderMock
                .Setup(x => x.GetDirectionsByRouteIdAsync(_routeId))
                .ReturnsAsync(() => _directionLogicMocks.Select(dl => dl.Object));
            _routeLogic = new RouteLogic(_routeId, _routeName, _serviceProvider);
            _routeLogicProviderMock
                .Setup(x => x.DepartureRoutes)
                .Returns(() => new IRouteLogic[]
                {
                    _routeLogic
                });
        }

        [Fact]
        public void RouteId_WhenCalled_ReturnsSetValue_Test() => Assert.True(_routeLogic.RouteId == _routeId);
        [Fact]
        public void RouteName_WhenCalled_ReturnsSetValue_Test() => Assert.True(_routeLogic.RouteName == _routeName);
        [Fact]
        public void GetStartStations_WhenCalled_ReturnsFirstStation_Test() =>
            Assert.Equal(new IStationLogic[] { _stationLogicMocks[0].Object }, _routeLogic.GetStartStations().ToArray());
        [Fact]
        public void GetNextStationsOf_WhenCalledWithNullParam_ThrowsArgumentNullException_Test() =>
            Assert.Throws<ArgumentNullException>(() => _routeLogic.GetNextStationsOf(null!));
        [Fact]
        public void GetNextStationsOf_WhenCalledWithUnknownStation_ThrowsArgumentException_Test()
        {
            var unknown = new StationLogic(_slLogger, new Station());
            Assert.Throws<ArgumentException>(() => _routeLogic.GetNextStationsOf(unknown));
        }
        [Fact]
        public void GetNextStationsOf_WhenCalled_ReturnsNextStations_Test() =>
            Assert.Equal(new IStationLogic[] { _stationLogicMocks[1].Object }, _routeLogic.GetNextStationsOf(_stationLogicMocks[0].Object).ToArray());

        public void Dispose() => _serviceProvider.Dispose();
    }
}
