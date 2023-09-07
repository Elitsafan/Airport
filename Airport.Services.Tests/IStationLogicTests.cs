using Airport.Models.Entities;
using Airport.Models.Enums;
using Airport.Models.Interfaces;
using Airport.Services.Logics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Airport.Services.Tests
{
    public class IStationLogicTests : IDisposable
    {
        private ServiceProvider _serviceProvider;

        public IStationLogicTests()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            serviceCollection.AddScoped<IStationRepository>((factory) => new Mock<IStationRepository>().Object);
            _serviceProvider = serviceCollection.BuildServiceProvider();

        }

        [Fact]
        public async Task EnterStation_StationOccupied_ThrowsTaskCanceledException_Test()
        {

            var logger = _serviceProvider.GetRequiredService<ILogger<StationLogic>>();
            var departureLogic = new Mock<IFlightLogic>();
            var landingLogic = new Mock<IFlightLogic>();
            var departure = new Departure();
            var landing = new Landing();
            var station = new Station();
            var cts = new CancellationTokenSource();

            departureLogic.SetupGet(x => x.Flight).Returns(departure);
            landingLogic.SetupGet(x => x.Flight).Returns(landing);
            var stationLogic = new StationLogic(_serviceProvider, logger, station);

            await stationLogic.SetFlightAsync(departureLogic.Object, source: cts);
            Assert.True(stationLogic.CurrentFlightType == FlightType.Departure);
            await Assert.ThrowsAsync<TaskCanceledException>(() => stationLogic.SetFlightAsync(landingLogic.Object, source: cts));
        }
        [Fact]
        public async Task EnterStation_FlightEntrance_StationOccupied_Test()
        {

            var logger = _serviceProvider.GetRequiredService<ILogger<StationLogic>>();
            var flightLogic = new Mock<IFlightLogic>();
            var flight = new Departure();
            var station = new Station();

            flightLogic.SetupGet(x => x.Flight).Returns(flight);
            IStationLogic stationLogic = new StationLogic(_serviceProvider, logger, station);

            Assert.False(stationLogic.CurrentFlightType.HasValue);
            await stationLogic.SetFlightAsync(flightLogic.Object);
            Assert.True(stationLogic.CurrentFlightType == FlightType.Departure);
        }

        public async void Dispose() => await _serviceProvider.DisposeAsync();
    }
}
