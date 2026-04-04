using Arpg.Application.Repositories;
using Arpg.Core.Models;
using Arpg.Core.Models.Customer;
using Arpg.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Arpg.Infrastructure.Repositories;

public class UserRepository(AppDbContext db) : Repository<User>(db), IUserRepository
{
    private readonly AppDbContext _db = db;

    public Task<bool> AnyAsync(Guid id)
    {
        return _db.Users.AnyAsync(u => u.Id == id);
    }



    public async Task<User?> GetAsync(Guid id)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
    }
    
    public async Task<User?> GetCurrentReadOnlyAsync(Guid id)
    {
        return await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
    }
    
    public async Task<User?> GetCurrentAsync(Guid id)
    {
        return await _db.Users.FindAsync(id);
    }
}