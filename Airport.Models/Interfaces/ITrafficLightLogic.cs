using Airport.Models.Enums;

namespace Airport.Models.Interfaces
{
    public interface ITrafficLightLogic
    {
        /// <summary>
        /// Gets the <see cref="IStationLogic"/> that associates with the traffic light
        /// </summary>
        /// <param name="routeId">The route whom <see langword="this"/> <see cref="ITrafficLightLogic"/> belongs to</param>
        /// <returns><see cref="IStationLogic"/> that stadns before <see langword="this"/> traffic light/> </returns>
        IEnumerable<IStationLogic?> this[int routeId] { get; }
        /// <summary>
        /// Checks if there is any other flight standing by on any of the stations near the traffic light
        /// </summary>
        /// <param name="stationLogic"></param>
        /// <returns></returns>
        bool IsAnyOtherFlightStandingBy(IStationLogic stationLogic);
        /// <summary>
        /// Checks if there is any other flight standing by that its value not equals to <paramref name="flightType"/>
        /// </summary>
        /// <param name="flightType"></param>
        /// <returns></returns>
        bool IsAnyOtherFlightStandingBy(FlightType flightType);
        /// <summary>
        /// Gets the station id that associates with the traffic light
        /// </summary>
        int StationId { get; }
        int TrafficLightId { get; }
    }
}