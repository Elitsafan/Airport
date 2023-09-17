using Airport.Models.Entities;
using Airport.Models.Interfaces;
using Airport.Services.Logics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics.CodeAnalysis;

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
            _station = new Station { StationId = 1 };
            _flightLogicMock = new Mock<IFlightLogic>();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            serviceCollection.AddScoped<IStationRepository>((factory) => new Mock<IStationRepository>().Object);
            serviceCollection.AddScoped<IStationFlightRepository>((factory) => new Mock<IStationFlightRepository>().Object);
            serviceCollection.AddScoped<IFlightLogic>((logic) => _flightLogicMock.Object);
            _serviceProvider = serviceCollection.BuildServiceProvider();

            _slLogger = _serviceProvider.GetRequiredService<ILogger<StationLogic>>();
            _stationLogic = new StationLogic(_serviceProvider, _slLogger, _station);
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
        [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "<Pending>")]
        public async Task EnterStation_EnterToAnotherStation_ThrowsOperationCanceledException_Test()
        {
            IStationLogic anotherStationLogic = new StationLogic(_serviceProvider, _slLogger, new Station { StationId = 2 });
            await _stationLogic.SetFlightAsync(_flightLogic, _cts);
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => anotherStationLogic.SetFlightAsync(_flightLogic, _cts));
        }

        [Fact]
        [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "<Pending>")]
        public async Task EnterStation_FlightEntrance_CurrentFlightTypeHasValue_Test()
        {
            Assert.False(_stationLogic.CurrentFlightType.HasValue);
            await _stationLogic.SetFlightAsync(_flightLogic);
            Assert.True(_stationLogic.CurrentFlightType.HasValue);
        }

        [Fact]
        [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "<Pending>")]
        public async Task EnterStation_FlightEntrance_CurrentFlightIdHasValue_Test()
        {
            Assert.False(_stationLogic.CurrentFlightId.HasValue);
            await _stationLogic.SetFlightAsync(_flightLogic);
            Assert.True(_stationLogic.CurrentFlightId.HasValue);
        }

        [Fact]
        [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "<Pending>")]
        public async Task EnterStationEvent_FlightEntered_StationChangedEventRaised_Test()
        {
            _stationLogic.StationChanged += OnStationChanged;
            await _stationLogic.SetFlightAsync(_flightLogic, _cts);
            Assert.True(_onStationChangedEventRaised);
            Assert.True(_countStationChangedRaised == 1);
            _stationLogic.StationChanged -= OnStationChanged;
        }

        [Fact]
        [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "<Pending>")]
        public async Task ClearAsync_FlightExited_StationEvacuated_Test()
        {
            await _stationLogic.SetFlightAsync(_flightLogic, _cts);
            Assert.NotNull(_stationLogic.CurrentFlightId);
            Assert.NotNull(_stationLogic.CurrentFlightType);
            await _stationLogic.ClearAsync();
            Assert.Null(_stationLogic.CurrentFlightId);
            Assert.Null(_stationLogic.CurrentFlightType);
        }

        [Fact]
        [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "<Pending>")]
        public async Task ClearAsync_ClearEmptyStation_ThrowsInvalidOperationException_Test()
        {
            Assert.Null(_stationLogic.CurrentFlightId);
            Assert.Null(_stationLogic.CurrentFlightType);
            await Assert.ThrowsAsync<InvalidOperationException>(_stationLogic.ClearAsync);
        }


        [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "<Pending>")]
        private async Task OnStationChanged(object? sender, Models.EventArgs.StationChangedEventArgs args)
        {
            _onStationChangedEventRaised = true;
            _countStationChangedRaised++;
            await Task.CompletedTask;
        }

        public void Dispose() => _serviceProvider.Dispose();
    }
}
