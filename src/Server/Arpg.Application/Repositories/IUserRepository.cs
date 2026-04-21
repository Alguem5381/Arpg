using Arpg.Core.Models.Customer;

namespace Arpg.Application.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<bool> AnyAsync(string username);
    Task<User?> GetAsync(Guid id);
}