using Arpg.Application.Queries;
using Arpg.Contracts.Dto.User;
using Arpg.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Arpg.Infrastructure.Queries;

public class UserQueries(AppDbContext db) : IUserQueries
{
    public async Task<UserInformationDto?> GetSimpleAsync(string username)
    {
        return await db.Users
            .AsNoTracking()
            .Where(u => u.Username == username)
            .Select(u => new UserInformationDto(u.DisplayName, u.Username))
            .FirstOrDefaultAsync();
    }

    public async Task<UserDto?> GetSelfAsync(Guid userId)
    {
        return await db.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new UserDto(u.DisplayName, u.Username))
            .FirstOrDefaultAsync();
    }
}