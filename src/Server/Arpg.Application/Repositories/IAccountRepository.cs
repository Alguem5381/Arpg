using Arpg.Core.Models.Customer;

namespace Arpg.Application.Repositories;

public interface IAccountRepository : IRepository<Account>
{
    Task<bool> AnyAsync(Guid id);
    Task<bool> AnyAsync(string email);
    Task<bool> AnyOwnerAsync(Guid ownerId);
    Task<Account?> GetAsync(Guid id);
    Task<Account?> GetAsync(string email);
    Task<Account?> GetOwnerAsync(Guid ownerId);
    Task<Account?> GetOwnerReadOnlyAsync(Guid ownerId);
    Task<Account?> GetReadOnlyAsync(Guid id);
    Task<Account?> GetReadOnlyAsync(string email);
}