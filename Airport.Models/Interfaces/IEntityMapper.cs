namespace Airport.Models.Interfaces
{
    public interface IEntityMapper<TEntity, TModel> : IDisposable
    {
        TEntity Map(TModel model);
        TModel Map(TEntity entity);
    }
}
