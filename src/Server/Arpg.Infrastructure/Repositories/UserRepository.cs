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
}