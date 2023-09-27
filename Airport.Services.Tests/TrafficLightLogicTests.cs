using System.Linq.Expressions;

namespace Airport.Services.Tests
{
    public class TrafficLightLogicTests : IDisposable
    {
        #region Fields
        private ServiceProvider _serviceProvider;
        private ILogger<StationLogic> _slLogger;
        private Mock<IStationLogic>[] _stationLogics;
        private ITrafficLightLogic _trafficLightLogic;
        private TrafficLight _trafficLight;
        private Mock<IStationLogicProvider> _stationLogicProviderMock;
        private Mock<IDirectionLogicProvider> _directionLogicProviderMock;
        private Mock<IRouteRepository> _routeRepository;
        private Mock<IFlightLogic> _flightLogicMock;
        private ObjectId[] _ids;
        private ObjectId _trafficLightId1;
        private Route[] _routes;
        #endregion

        public TrafficLightLogicTests()
        {
            _ids = new ObjectId[]
            {
                ObjectId.Parse("000000000000000000000001"),
                ObjectId.Parse("000000000000000000000002"),
                ObjectId.Parse("000000000000000000000003")
            };
            _stationLogicProviderMock = new Mock<IStationLogicProvider>();
            _directionLogicProviderMock = new Mock<IDirectionLogicProvider>();
            _routeRepository = new Mock<IRouteRepository>();
            _flightLogicMock = new Mock<IFlightLogic>();
            _flightLogicMock.SetupGet(x => x.Flight).Returns(new Departure());
            //_flightLogicMock.Setup
            //var mocks = new List<Mock<IStationLogic>>
            //{
            //    new Mock<IStationLogic>(),
            //    new Mock<IStationLogic>(),
            //    new Mock<IStationLogic>()
            //};
            //mocks[0].SetupGet(x => x.StationId).Returns(1);
            //mocks[0].SetupGet(x => x.StationId).Returns(2);
            //mocks[0].SetupGet(x => x.StationId).Returns(3);
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<ILogger<StationLogic>>(factory => new Mock<ILogger<StationLogic>>().Object);
            serviceCollection.AddSingleton<IStationLogicProvider>(factory => _stationLogicProviderMock.Object);
            serviceCollection.AddSingleton<IDirectionLogicProvider>(factory => _directionLogicProviderMock.Object);
            serviceCollection.AddScoped<IRouteRepository>((factory) => new Mock<IRouteRepository>().Object);
            serviceCollection.AddScoped<IStationRepository>((factory) => new Mock<IStationRepository>().Object);
            serviceCollection.AddScoped<IFlightLogic>(logic => _flightLogicMock.Object);
            //serviceCollection.AddScoped<IRouteRepository>((factory) => _routeRepository.Object);
            _serviceProvider = serviceCollection.BuildServiceProvider();

            _trafficLight = new TrafficLight
            {
                StationId = _ids[1],
                TrafficLightId = _trafficLightId1,
            };
            _routes = new[]
            {
                new Route
                {
                    RouteId = new ObjectId("650abb1ee574435a814d7ec0"),
                    RouteName = "Landing",
                    Directions = new List<Direction>
                    {
                        new Direction
                        {
                            From = _ids[0],
                            To = _ids[1],
                        }
                    }
                },
                new Route
                {
                    RouteId = new ObjectId("650abb1ee574435a814d7ec"),
                    RouteName = "Departure",
                    Directions = new List<Direction>
                    {
                        new Direction
                        {
                            From = _ids[2],
                            To = _ids[1],
                        }
                    }
                }
            };
            _slLogger = _serviceProvider.GetRequiredService<ILogger<StationLogic>>();
            _stationLogics = new Mock<IStationLogic>[]
            {
                new Mock<IStationLogic>(),
                new Mock<IStationLogic>(),
                new Mock<IStationLogic>(),
            };
            _stationLogics[0]
                .SetupGet(x => x.StationId)
                .Returns(_ids[0]);
            _stationLogics[1]
                .SetupGet(x => x.StationId)
                .Returns(_ids[1]);
            _stationLogics[2]
                .SetupGet(x => x.StationId)
                .Returns(_ids[2]);
            _stationLogicProviderMock
                .Setup(x => x.GetAll())
                .Returns(() => new IStationLogic[]
                {
                    _stationLogics[0].Object,
                    _stationLogics[1].Object,
                    _stationLogics[2].Object,
                });
            _stationLogicProviderMock
                .Setup(x => x.FindBy(It.IsAny<Expression<Func<IStationLogic, bool>>>()))
                .Returns(() => new IStationLogic[] { _stationLogics[0].Object });
            _trafficLightLogic = new TrafficLightLogic(_serviceProvider, _trafficLight);
        }


        [Fact]
        public void IsAnyOtherFlightStandingBy_NoFlightStandingby_ReturnsFalse_Test()
        {
            var slp = _serviceProvider.GetRequiredService<IStationLogicProvider>();
            var sl3 = slp.GetAll().First(sl => sl.StationId == _ids[2]);
            var result = _trafficLightLogic.IsAnyOtherFlightStandingBy(sl3);
            Assert.False(result);
        }

        [Fact]
        public async Task IsAnyOtherFlightStandingBy_FlightStandingby_ReturnsTrue_Test()
        {
            var slp = _serviceProvider.GetRequiredService<IStationLogicProvider>();
            var sl3 = slp.GetAll().First(sl => sl.StationId == _ids[2]);
            var sl1 = slp.GetAll().First(sl => sl.StationId == _ids[0]);
            var flightLogic = _serviceProvider
                .CreateAsyncScope()
                .ServiceProvider
                .GetRequiredService<IFlightLogic>();
            await sl1.SetFlight(flightLogic);
            _stationLogics[0]
                .SetupGet(x => x.CurrentFlightType)
                .Returns(Models.Enums.FlightType.Landing);
            _stationLogics[0]
                .SetupGet(x => x.CurrentFlightId)
                .Returns(It.IsAny<ObjectId>());
            var result = _trafficLightLogic.IsAnyOtherFlightStandingBy(sl3);
            Assert.True(result);
        }

        //[Fact]
        //public void IsAnyOtherFlightStandingBy_FlightStandingbyCheckedByType_ReturnsTrue_Test()
        //{
        //    // TODO
        //}

        //[Fact]
        //public void IsAnyOtherFlightStandingBy_NoFlightStandingbyCheckedByType_ReturnsFalse_Test()
        //{
        //    // TODO
        //}

        public void Dispose() => _serviceProvider.Dispose();
    }
}
