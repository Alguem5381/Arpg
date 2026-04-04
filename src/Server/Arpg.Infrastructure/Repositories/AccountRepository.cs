using Arpg.Application.Repositories;
using Arpg.Core.Models;
using Arpg.Core.Models.Customer;
using Arpg.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Arpg.Infrastructure.Repositories;

public class AccountRepository(AppDbContext db) : Repository<Account>(db), IAccountRepository
{
    private readonly AppDbContext _db = db;

    public async Task<Account?> GetAsync(Guid id)
        => await _db.Accounts.FirstOrDefaultAsync(account => account.Id == id);

    public async Task<Account?> GetByUserIdAsync(Guid userId)
        => await _db.Accounts.FirstOrDefaultAsync(account => account.UserId == userId);

    public Task<bool> AnyAsync(string value)
        => _db.Accounts.AnyAsync(a => a.Username == value || a.Email == value);

    public async Task<Account?> GetReadOnlyAsync(string username)
        => await _db.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Username == username);
}