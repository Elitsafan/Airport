using Airport.Models.Enums;

namespace Airport.Models.Interfaces
{
    public interface IFlightLauncherService : IDisposable
    {
        Task<HttpResponseMessage> StartAsync();
        Task LaunchManyAsync(params string[]? args);
        Task<HttpResponseMessage[]> LaunchManyAsync();
        Task SetFlightTimeoutAsync(TimeSpan timeout, FlightType? flightType = null);
        Task<HttpResponseMessage> LaunchOneAsync(FlightType flightType);
    }
}
