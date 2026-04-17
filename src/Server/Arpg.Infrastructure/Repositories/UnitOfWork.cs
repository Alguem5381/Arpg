using Arpg.Application.Repositories;
using Arpg.Infrastructure.Data;

namespace Arpg.Infrastructure.Repositories;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public async Task CommitAsync()
    {
        await context.SaveChangesAsync();
    }
}