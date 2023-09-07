namespace Airport.Models.Interfaces
{
    public interface IRepository<T> : IDisposable where T : class
    {
        IQueryable<T> GetAll();
    }
}
