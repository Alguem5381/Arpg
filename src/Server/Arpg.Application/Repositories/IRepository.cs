namespace Arpg.Application.Repositories;

public interface IRepository<in TEntity> where TEntity : class
{
    void Add(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
}