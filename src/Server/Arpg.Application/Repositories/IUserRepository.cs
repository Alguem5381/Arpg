using Arpg.Core.Models.Customer;

namespace Arpg.Application.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<bool> AnyAsync(string username);
    Task<User?> GetAsync(Guid id);
    Task<User?> GetAsync(string username);
    Task<bool> AnyAll(IEnumerable<Guid> playerIds);
    Task<List<Guid>> FilterExistingAsync(IEnumerable<Guid> userIds);
}