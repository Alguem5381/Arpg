using Arpg.Core.Models.Customer;

namespace Arpg.Application.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<bool> AnyAsync(Guid id);
    Task<bool> AnyAsync(string username);
    Task<User?> GetAsync(Guid id);
    Task<User?> GetAsync(string username);
    Task<User?> GetReadOnlyAsync(Guid id);
    Task<User?> GetReadOnlyAsync(string username);
}