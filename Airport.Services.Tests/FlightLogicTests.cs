namespace Airport.Services.Tests
{
    public class FlightLogicTests : IDisposable
    {
        private ServiceProvider _serviceProvider;
        private ILogger<StationLogic> _slLogger;
        private IFlightLogic _flightLogic;
        private Mock<IFlightRepository> _flightRepositoryMock;
        private Mock<IRouteRepository> _routeRepositoryMock;
        private Mock<IStationRepository> _stationRepositoryMock;
        private Mock<IRouteLogic> _routeLogicMock;
        private Mock<IRouteLogicProvider> _routeLogicProviderMock;
        private Mock<IStationLogicProvider> _stationLogicProviderMock;
        private Mock<IDirectionLogicProvider> _directionLogicProviderMock;
        private Mock<ITrafficLightLogicProvider> _trafficLightLogicProviderMock;
        private bool _onFlightRunDoneEventRaised;
        private ObjectId _routeLogicId;

        public FlightLogicTests()
        {
            _onFlightRunDoneEventRaised = false;
            _routeLogicId = new ObjectId("650abb1ee574435a814d7ec1");
            _flightRepositoryMock = new Mock<IFlightRepository>();
            _routeRepositoryMock = new Mock<IRouteRepository>();
            _stationRepositoryMock = new Mock<IStationRepository>();
            _routeLogicMock = new Mock<IRouteLogic>();
            _routeLogicProviderMock = new Mock<IRouteLogicProvider>();
            _stationLogicProviderMock = new Mock<IStationLogicProvider>();
            _directionLogicProviderMock = new Mock<IDirectionLogicProvider>();
            _trafficLightLogicProviderMock = new Mock<ITrafficLightLogicProvider>();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<ILogger<IFlightLogic>>(factory => new Mock<ILogger<IFlightLogic>>().Object);
            serviceCollection.AddSingleton<ILogger<StationLogic>>(factory => new Mock<ILogger<StationLogic>>().Object);
            serviceCollection.AddSingleton<IRouteLogicProvider>(factory => _routeLogicProviderMock.Object);
            serviceCollection.AddSingleton<IStationLogicProvider>(factory => _stationLogicProviderMock.Object);
            serviceCollection.AddSingleton<IDirectionLogicProvider>(factory => _directionLogicProviderMock.Object);
            serviceCollection.AddSingleton<ITrafficLightLogicProvider>(factory => _trafficLightLogicProviderMock.Object);
            serviceCollection.AddScoped<IFlightRepository>(factory => _flightRepositoryMock.Object);
            serviceCollection.AddScoped<IRouteRepository>(factory => _routeRepositoryMock.Object);
            serviceCollection.AddScoped<IStationRepository>(factory => _stationRepositoryMock.Object);
            _serviceProvider = serviceCollection.BuildServiceProvider();

            _slLogger = _serviceProvider.GetRequiredService<ILogger<StationLogic>>();
            var stationLogics = new IStationLogic[]
            {
                new StationLogic(_slLogger, new Station { StationId = ObjectId.GenerateNewId() }),
                new StationLogic(_slLogger, new Station { StationId = ObjectId.GenerateNewId() }),
                new StationLogic(_slLogger, new Station { StationId = ObjectId.GenerateNewId() })
            };
            _routeLogicMock
                .SetupGet(x => x.RouteId)
                .Returns(_routeLogicId);
            _routeLogicMock
                .SetupGet(x => x.RouteName)
                .Returns("Departure");
            _routeLogicMock
                .Setup(x => x.GetStartStations())
                .Returns(() => new IStationLogic[]
                {
                    stationLogics[0]
                });
            _routeLogicMock
                .SetupSequence(x => x.GetRightOfWay(null, stationLogics[0]!, default))
                .ReturnsAsync(() => default);
            _routeLogicMock
                .SetupSequence(x => x.GetRightOfWay(stationLogics[0]!, stationLogics[1]!, default))
                .ReturnsAsync(() => default);
            _routeLogicMock
                .SetupSequence(x => x.GetRightOfWay(stationLogics[1], stationLogics[2]!, default))
                .ReturnsAsync(() => default);
            _routeLogicMock
                .Setup(x => x.StartRun())
                .ReturnsAsync(() => default);
            _routeLogicProviderMock
                .Setup(x => x.DepartureRoutes)
                .Returns(() => new IRouteLogic[]
                {
                    _routeLogicMock.Object
                }.AsEnumerable());
            Flight flight = new Departure();
            var logger = _serviceProvider.GetRequiredService<ILogger<IFlightLogic>>();
            var routeLogicProvider = _serviceProvider.GetRequiredService<IRouteLogicProvider>();
            var routeLogic = routeLogicProvider.DepartureRoutes.First();
            var flightRepository = _serviceProvider
                .CreateAsyncScope()
                .ServiceProvider
                .GetRequiredService<IFlightRepository>();
            _flightLogic = new FlightLogic(flightRepository, routeLogic, logger, flight);
        }

        [Fact]
        public void Flight_FlightLogicConstructed_ReturnsFlight_Test() => Assert.NotNull(_flightLogic.Flight);

        [Fact]
        public void RouteId_FlightLogicConstructed_ReturnsValidValue_Test() => Assert.True(_flightLogic.RouteId != ObjectId.Empty);

        [Fact]
        public void CurrentStation_FlightLogicConstructed_ReturnsNull_Test() => Assert.Null(_flightLogic.CurrentStation);

        [Fact]
        public async Task FlightRunDoneEvent_FlightRunDone_FlightRunDoneEventRaised_Test()
        {
            _flightLogic.FlightRunDone += OnFlightRunDone;
            await _flightLogic.Run();
            Assert.True(_onFlightRunDoneEventRaised);
            _flightLogic.FlightRunDone -= OnFlightRunDone;
        }

        [Fact]
        public async Task Run_Executed_FlightStartRunning_Test()
        {
            await _flightLogic.Run();
            Assert.NotNull(_flightLogic.Flight);
        }

        [Fact]
        public async Task ThrowIfCancellationRequested_WhenCalledSecondly_ThrowsOperationCancelledException_Test()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            await _flightLogic.ThrowIfCancellationRequested(cancellationTokenSource);
            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => _flightLogic.ThrowIfCancellationRequested(cancellationTokenSource));
        }


        // Impossible to test event raised with Assert.RaisesAnyAsync().
        // AsyncEventHandler type is not supported.
        private async Task OnFlightRunDone(object? sender, Models.EventArgs.FlightRunDoneEventArgs args)
        {
            _onFlightRunDoneEventRaised = true;
            await Task.CompletedTask;
        }

        public void Dispose() => _serviceProvider.Dispose();
    }
}
