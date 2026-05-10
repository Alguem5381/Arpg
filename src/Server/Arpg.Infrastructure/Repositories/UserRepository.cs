using Arpg.Application.Repositories;
using Arpg.Core.Models.Customer;
using Arpg.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Arpg.Infrastructure.Repositories;

public class UserRepository(AppDbContext db) : Repository<User>(db), IUserRepository
{
    private readonly AppDbContext _db = db;

    public Task<bool> AnyAsync(string username)
        => _db.Users.AnyAsync(user => user.Username == username);

    public async Task<User?> GetAsync(Guid id)
        => await _db.Users.FirstOrDefaultAsync(user => user.Id == id);

    public async Task<User?> GetAsync(string username)
        => await _db.Users.FirstOrDefaultAsync(user => user.Username == username);

    public async Task<bool> AnyAll(IEnumerable<Guid> playerIds)
        => await _db.Users.CountAsync(u => playerIds.Contains(u.Id)) == playerIds.Distinct().Count();

    public async Task<List<Guid>> FilterExistingAsync(IEnumerable<Guid> userIds)
        => await _db.Users
            .Where(u => userIds.Contains(u.Id))
            .Select(u => u.Id)
            .ToListAsync();
}