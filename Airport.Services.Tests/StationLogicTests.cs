namespace Airport.Services.Tests
{
    public class StationLogicTests : IDisposable
    {
        #region Fields
        private ServiceProvider _serviceProvider;
        private ILogger<StationLogic> _slLogger;
        private Station _station;
        private IFlightLogic _flightLogic;
        private IStationLogic _stationLogic;
        private Mock<IFlightLogic> _flightLogicMock;
        private CancellationTokenSource _cts;
        private bool _onStationChangedEventRaised;
        private int _countStationChangedRaised;
        #endregion

        public StationLogicTests()
        {
            _cts = new CancellationTokenSource();
            _onStationChangedEventRaised = false;
            _countStationChangedRaised = 0;
            _station = new Station { StationId = new ObjectId("000000000000000000000001") };
            _flightLogicMock = new Mock<IFlightLogic>();
            _flightLogicMock
                .SetupGet(x => x.CurrentStation)
                .Returns(() => new Mock<IStationLogic>().Object);

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<ILogger<StationLogic>>(factory => new Mock<ILogger<StationLogic>>().Object);
            serviceCollection.AddScoped<IStationRepository>(factory => new Mock<IStationRepository>().Object);
            serviceCollection.AddScoped<IFlightLogic>(logic => _flightLogicMock.Object);
            _serviceProvider = serviceCollection.BuildServiceProvider();

            _slLogger = _serviceProvider.GetRequiredService<ILogger<StationLogic>>();
            _stationLogic = new StationLogic(_slLogger, _station);
            _flightLogicMock
                .SetupGet(x => x.Flight)
                .Returns(new Departure());
            _flightLogic = _serviceProvider
                .CreateAsyncScope()
                .ServiceProvider
                .GetRequiredService<IFlightLogic>();
            _flightLogicMock
                .SetupSequence(x => x.ThrowIfCancellationRequested(_cts))
                .Returns(() => Task.CompletedTask)
                .ThrowsAsync(new OperationCanceledException());
        }

        [Fact]
        public async Task EnterStation_EnterToAnotherStation_ThrowsOperationCanceledException_Test()
        {
            IStationLogic anotherStationLogic = new StationLogic(
                _slLogger, 
                new Station { StationId = new ObjectId("000000000000000000000002") });
            await _stationLogic.SetFlight(_flightLogic, _cts);
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => anotherStationLogic.SetFlight(_flightLogic, _cts));
        }

        [Fact]
        public async Task EnterStation_FlightEntrance_CurrentFlightTypeHasValue_Test()
        {
            Assert.False(_stationLogic.CurrentFlightType.HasValue);
            await _stationLogic.SetFlight(_flightLogic);
            Assert.True(_stationLogic.CurrentFlightType.HasValue);
        }

        [Fact]
        public async Task EnterStation_FlightEntrance_CurrentFlightIdHasValue_Test()
        {
            Assert.False(_stationLogic.CurrentFlightId.HasValue);
            await _stationLogic.SetFlight(_flightLogic);
            Assert.True(_stationLogic.CurrentFlightId.HasValue);
        }

        [Fact]
        public async Task EnterStationEvent_FlightEntered_StationChangedEventRaised_Test()
        {
            _stationLogic.StationChanged += OnStationChanged;
            await _stationLogic.SetFlight(_flightLogic, _cts);
            Assert.True(_onStationChangedEventRaised);
            Assert.True(_countStationChangedRaised == 1);
            _stationLogic.StationChanged -= OnStationChanged;
        }

        [Fact]
        public async Task Clear_FlightExited_StationEvacuated_Test()
        {
            await _stationLogic.SetFlight(_flightLogic, _cts);
            Assert.NotNull(_stationLogic.CurrentFlightId);
            Assert.NotNull(_stationLogic.CurrentFlightType);
            await _stationLogic.Clear();
            Assert.Null(_stationLogic.CurrentFlightId);
            Assert.Null(_stationLogic.CurrentFlightType);
        }

        [Fact]
        public async Task Clear_ClearEmptyStation_ThrowsInvalidOperationException_Test()
        {
            Assert.Null(_stationLogic.CurrentFlightId);
            Assert.Null(_stationLogic.CurrentFlightType);
            await Assert.ThrowsAsync<InvalidOperationException>(_stationLogic.Clear);
        }


        private async Task OnStationChanged(object? sender, Models.EventArgs.StationChangedEventArgs args)
        {
            _onStationChangedEventRaised = true;
            _countStationChangedRaised++;
            await Task.CompletedTask;
        }

        public void Dispose() => _serviceProvider.Dispose();
    }
}
