using Airport.Models;
using Airport.Models.Enums;
using Airport.Models.EventArgs;
using Airport.Models.Interfaces;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Airport.Services
{
    public class AirportHubHandlerRegistrar : IAirportHubHandlerRegistrar
    {
        #region Fields
        private readonly IHubContext<AirportHub> _hub;
        private readonly IStationLogicProvider _stationLogicProvider;
        private JsonSerializerSettings _jsonSerializerSettings = null!;
        private readonly IQueryable<StationChangedData> _stations;
        #endregion

        public AirportHubHandlerRegistrar(
            IStationLogicProvider stationLogicProvider,
            IHubContext<AirportHub> hub)
        {
            _hub = hub;
            _stationLogicProvider = stationLogicProvider;
            // Serializer settings 
            _jsonSerializerSettings = new()
            {
                Formatting = Formatting.Indented,
                ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() },
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
            };

            // Prepare stations query for sending the state of stations
            _stations = _stationLogicProvider
                .GetAll()
                .AsEnumerable()
                .OrderBy(s => s.StationId)
                .Select(s => new StationChangedData
                {
                    StationId = s.StationId,
                    Flight = s.CurrentFlightId is null
                    ? null
                    : new FlightInfo
                    {
                        FlightId = s.CurrentFlightId,
                        FlightType = s.CurrentFlightType
                    },
                })
                .AsQueryable();
        }

        public void Initialize() => RegisterStationChanged(_stationLogicProvider.GetAll());

        private void RegisterStationChanged(IEnumerable<IStationLogic> stationLogics)
        {
            foreach (var stationLogic in stationLogics)
                stationLogic.StationChanged += OnStationChangedAsync;
        }
        private async Task OnStationChangedAsync(object? sender, StationChangedEventArgs e)
        {
            try
            {
                await _hub.Clients.All.SendAsync(
                nameof(IStationLogic.StationChanged),
                    JsonConvert.SerializeObject(_stations, _jsonSerializerSettings));
            }
            catch (Exception ex)
            {
                await Task.FromException(ex);
            }
        }

        private class StationChangedData
        {
            public ObjectId StationId { get; set; }
            public FlightInfo? Flight { get; set; }
        }
        private class FlightInfo
        {
            public ObjectId? FlightId { get; set; }
            public FlightType? FlightType { get; set; }
        }
    }
}
