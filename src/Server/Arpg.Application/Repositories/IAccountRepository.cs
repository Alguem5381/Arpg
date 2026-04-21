using Arpg.Core.Models.Customer;

namespace Arpg.Application.Repositories;

public interface IAccountRepository : IRepository<Account>
{
    Task<bool> AnyAsync(string email);
    Task<Account?> GetAsync(Guid id);
    Task<Account?> GetAsync(string email);
    Task<Account?> GetOwnerAsync(Guid ownerId);
}