namespace Airport.Models.Interfaces
{
    public interface IAirportDbContextSetup
    {
        Task DropDatabaseAsync();
        Task SeedDatabaseAsync();
    }
}
