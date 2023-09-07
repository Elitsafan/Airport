namespace Airport.Models.Interfaces
{
    public interface IEntityMapper<TEntity, TModel> : IDisposable
    {
        Task<TEntity> MapAsync(TModel model);
        TModel Map(TEntity entity);
    }
}
