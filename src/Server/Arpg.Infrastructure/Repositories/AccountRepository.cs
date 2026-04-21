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

    public async Task<Account?> GetAsync(Guid id)
        => await _db.Accounts.FirstOrDefaultAsync(account => account.Id == id);

    public async Task<Account?> GetAsync(string email)
        => await _db.Accounts.FirstOrDefaultAsync(account => account.Email == email);

    public async Task<bool> AnyAsync(string email)
        => await _db.Accounts.AnyAsync(account => account.Email == email);
}