namespace Arpg.Application.Repositories;

public interface IUnitOfWork
{
    Task CommitAsync();
}