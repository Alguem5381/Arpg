using Arpg.Application.Repositories;
using Arpg.Infrastructure.Data;

namespace Arpg.Infrastructure.Repositories;

public class Repository<TEntity>(AppDbContext db) : IRepository<TEntity> where TEntity : class
{
    public void Add(TEntity entity)
    {
        db.Set<TEntity>().Add(entity);
    }

    public void Update(TEntity entity)
    {
        db.Set<TEntity>().Update(entity);
    }

    public void Delete(TEntity entity)
    {
        db.Set<TEntity>().Remove(entity);
    }
}