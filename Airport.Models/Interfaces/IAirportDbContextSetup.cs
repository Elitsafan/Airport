namespace Airport.Models.Interfaces
{
    public interface IAirportDbContextSetup
    {
        Task SeedAsync();
    }
}
