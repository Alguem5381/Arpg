using Arpg.Core.Models;
using Arpg.Core.Models.Customer;

namespace Arpg.Application.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<bool> AnyAsync(Guid id);
    Task<User?> GetAsync(Guid id);
    Task<User?> GetCurrentReadOnlyAsync(Guid id);
    Task<User?> GetCurrentAsync(Guid id);
}