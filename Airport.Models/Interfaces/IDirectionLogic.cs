namespace Airport.Models.Interfaces
{
    public interface IDirectionLogic
    {
        int DirectionId { get; }
        int? From { get; }
        int? To { get; }
        int? RouteId { get; }
    }
}
