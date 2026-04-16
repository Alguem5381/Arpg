using Arpg.Application.Repositories;
using Arpg.Core.Models.Customer;
using Arpg.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Arpg.Infrastructure.Repositories;

public class AccountRepository(AppDbContext db) : Repository<Account>(db), IAccountRepository
{
    private readonly AppDbContext _db = db;

    public async Task<Account?> GetOwnerAsync(Guid ownerId)
        => await _db.Accounts.FirstOrDefaultAsync(account => account.OwnerId == ownerId);

    public async Task<Account?> GetOwnerReadOnlyAsync(Guid ownerId)
        => await _db.Accounts.AsNoTracking().FirstOrDefaultAsync(account => account.OwnerId == ownerId);

    public async Task<Account?> GetAsync(Guid id)
        => await _db.Accounts.FirstOrDefaultAsync(account => account.Id == id);

    public async Task<Account?> GetAsync(string email)
        => await _db.Accounts.FirstOrDefaultAsync(account => account.Email == email);

    public async Task<Account?> GetReadOnlyAsync(Guid id)
        => await _db.Accounts.AsNoTracking().FirstOrDefaultAsync(account => account.Id == id);

    public async Task<Account?> GetReadOnlyAsync(string email)
        => await _db.Accounts.AsNoTracking().FirstOrDefaultAsync(account => account.Email == email);

    public Task<bool> AnyOwnerAsync(Guid ownerId)
        => _db.Accounts.AnyAsync(account => account.OwnerId == ownerId);

    public Task<bool> AnyAsync(Guid id)
        => _db.Accounts.AnyAsync(account => account.Id == id);

    public Task<bool> AnyAsync(string email)
        => _db.Accounts.AnyAsync(account => account.Email == email);
}