using Arpg.Application.Repositories;
using Arpg.Infrastructure.Data;
using FluentResults;

namespace Arpg.Infrastructure.Repositories;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public async Task CommitAsync()
    {
        await context.SaveChangesAsync();
    }
}