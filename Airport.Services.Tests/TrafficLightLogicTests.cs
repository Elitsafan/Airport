using Airport.Models.Entities;
using Airport.Models.Interfaces;
using Airport.Services.Logics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace Airport.Services.Tests
{
    public class TrafficLightLogicTests : IDisposable
    {
        #region Fields
        private ServiceProvider _serviceProvider;
        private ILogger<StationLogic> _slLogger;
        private ITrafficLightLogic _trafficLightLogic;
        private TrafficLight _trafficLight;
        private Mock<IStationLogicProvider> _stationLogicProviderMock;
        private IStationLogic _stationLogic;
        private Mock<IDirectionLogicProvider> _directionLogicProviderMock;
        private Mock<IRouteRepository> _routeRepository;
        private Mock<IFlightLogic> _flightLogicMock; 
        #endregion

        public TrafficLightLogicTests()
        {
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
            serviceCollection.AddLogging();
            serviceCollection.AddSingleton<IStationLogicProvider>(factory => _stationLogicProviderMock.Object);
            serviceCollection.AddSingleton<IDirectionLogicProvider>(factory => _directionLogicProviderMock.Object);
            serviceCollection.AddScoped<IStationRepository>((factory) => new Mock<IStationRepository>().Object);
            serviceCollection.AddScoped<IFlightLogic>(logic => _flightLogicMock.Object);
            //serviceCollection.AddScoped<IRouteRepository>((factory) => _routeRepository.Object);
            _serviceProvider = serviceCollection.BuildServiceProvider();

            _trafficLight = new TrafficLight
            {
                Station = new Station
                {
                    StationId = 2
                },
                StationId = 2,
                TrafficLightId = 1,
                Routes = new List<Route>
                {
                    new Route
                    {
                        RouteId = 1,
                        RouteName = "Landing",
                        Directions = new List<Direction>
                        {
                            new Direction
                            {
                                DirectionId = 1,
                                From = 1,
                                To = 2
                            }
                        }
                    },
                    new Route
                    {
                        RouteId = 2,
                        RouteName = "Departure",
                        Directions = new List<Direction>
                        {
                            new Direction
                            {
                                DirectionId = 2,
                                From = 3,
                                To = 2
                            }
                        }
                    }
                }
            };
            _slLogger = _serviceProvider.GetRequiredService<ILogger<StationLogic>>();
            var stationLogics = new IStationLogic[]
            {
                new StationLogic(_serviceProvider, _slLogger, new Station { StationId = 1 }),
                new StationLogic(_serviceProvider, _slLogger, new Station { StationId = 3 })
            };
            _stationLogic = new StationLogic(_serviceProvider, _slLogger, _trafficLight.Station);
            _stationLogicProviderMock
                .Setup(x => x.GetAll())
                .Returns(() => new IStationLogic[]
                {
                    stationLogics[0],
                    _stationLogic,
                    stationLogics[1],
                }.AsQueryable());
            _stationLogicProviderMock
                .Setup(x => x.FindBy(sl => It.Is<IStationLogic>(s => true).StationId == sl.StationId))
                .Returns(() => stationLogics.AsQueryable());
            _trafficLightLogic = new TrafficLightLogic(_serviceProvider, _trafficLight);
        }


        [Fact]
        public void IsAnyOtherFlightStandingBy_NoFlightStandingby_ReturnsFalse_Test()
        {
            var slp = _serviceProvider.GetRequiredService<IStationLogicProvider>();
            var sl3 = slp.GetAll().First(sl => sl.StationId == 3);
            var result = _trafficLightLogic.IsAnyOtherFlightStandingBy(sl3);
            Assert.False(result);
        }

        [Fact]
        [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "<Pending>")]
        public void IsAnyOtherFlightStandingBy_FlightStandingby_ReturnsTrue_Test()
        {
            //TODO

            //var slp = _serviceProvider.GetRequiredService<IStationLogicProvider>();
            //var sl3 = slp.GetAll().First(sl => sl.StationId == 3);
            //var sl1 = slp.GetAll().First(sl => sl.StationId == 1);
            //var flightLogic = _serviceProvider
            //    .CreateAsyncScope()
            //    .ServiceProvider
            //    .GetRequiredService<IFlightLogic>();
            //await sl1.SetFlightAsync(flightLogic);
            //var result = _trafficLightLogic.IsAnyOtherFlightStandingBy(sl3);
            //Assert.True(result);
        }

        [Fact]
        public void IsAnyOtherFlightStandingBy_FlightStandingbyCheckedByType_ReturnsTrue_Test()
        {
            //TODO
        }

        [Fact]
        public void IsAnyOtherFlightStandingBy_NoFlightStandingbyCheckedByType_ReturnsFalse_Test()
        {
            //TODO
        }

        public void Dispose() => _serviceProvider.Dispose();
    }
}
