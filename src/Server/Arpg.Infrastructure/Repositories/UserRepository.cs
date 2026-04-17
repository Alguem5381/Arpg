using Arpg.Application.Repositories;
using Arpg.Core.Models.Customer;
using Arpg.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Arpg.Infrastructure.Repositories;

public class UserRepository(AppDbContext db) : Repository<User>(db), IUserRepository
{
    private readonly AppDbContext _db = db;

    public Task<bool> AnyAsync(Guid id)
        => _db.Users.AnyAsync(u => u.Id == id);

    public Task<bool> AnyAsync(string username)
        => _db.Users.AnyAsync(u => u.Username == username);

    public async Task<User?> GetAsync(Guid id)
        => await _db.Users.FirstOrDefaultAsync(u => u.Id == id);

    public async Task<User?> GetAsync(string username)
        => await _db.Users.FirstOrDefaultAsync(u => u.Username == username);

    public async Task<User?> GetReadOnlyAsync(Guid id)
        => await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

    public async Task<User?> GetReadOnlyAsync(string username)
        => await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username);
}