using Arpg.Core.Models;
using Arpg.Core.Models.Customer;

namespace Arpg.Application.Repositories;

public interface IAccountRepository : IRepository<Account>
{
    Task<Account?> GetAsync(Guid id);
    Task<Account?> GetByUserIdAsync(Guid userId);
    Task<bool> AnyAsync(string value);
    Task<Account?> GetReadOnlyAsync(string username);
}