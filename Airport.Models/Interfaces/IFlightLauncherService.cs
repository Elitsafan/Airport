using Airport.Models.Enums;

namespace Airport.Models.Interfaces
{
    public interface IFlightLauncherService : IDisposable
    {
        Task<HttpResponseMessage> Start();
        IAsyncEnumerable<HttpResponseMessage?> LaunchMany(params string[]? args);
        IAsyncEnumerable<HttpResponseMessage> LaunchMany();
        Task SetFlightTimeout(FlightType? flightType = null);
        Task<HttpResponseMessage> LaunchOne(FlightType flightType);
    }
}
