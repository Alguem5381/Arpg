using Arpg.Application.Queries;
using Arpg.Contracts.Dto.GameTable;
using Arpg.Contracts.Dto.Sheet;
using Arpg.Contracts.Dto.User;
using Arpg.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Arpg.Infrastructure.Queries;

public class GameTableQueries(AppDbContext db) : IGameTableQueries
{
    public async Task<List<SimpleGameTableDto>> GetAllAsync(Guid ownerId)
        => await db.GameTables
            .AsNoTracking()
            .Where(table => table.DmId == ownerId)
            .Select(table =>
                new SimpleGameTableDto(table.Id, table.Name, table.DmId, table.TemplateId, table.CreatedAt))
            .ToListAsync();

    public async Task<List<UserInformationDto>> GetUsersAsync(Guid tableId, Guid userId)
    {
        var playerIds = await db.GameTables
            .AsNoTracking()
            .Where(table => table.Id == tableId && table.UserIds.Contains(userId))
            .Select(table => table.UserIds)
            .FirstOrDefaultAsync();

        if (playerIds is null || playerIds.Count == 0)
            return [];

        return await db.Users
            .AsNoTracking()
            .Where(user => playerIds.Contains(user.Id))
            .Select(user => new UserInformationDto(user.DisplayName, user.Username))
            .ToListAsync();
    }

    public async Task<List<SimpleSheetDto>> GetAllSheetsAsync(Guid tableId, Guid dmId)
    {
        var sheetIds = await db.GameTables
            .AsNoTracking()
            .Where(table => table.Id == tableId && table.DmId == dmId)
            .Select(table => table.SheetIds)
            .FirstOrDefaultAsync();

        if (sheetIds is null || sheetIds.Count == 0)
            return [];

        return await db.Sheets
            .AsNoTracking()
            .Where(sheet => sheetIds.Contains(sheet.Id))
            .Select(sheet => new SimpleSheetDto(sheet.Id, sheet.Name))
            .ToListAsync();
    }

    public async Task<List<SimpleSheetDto>> GetUserSheetsAsync(Guid tableId, Guid playerId)
    {
        var table = await db.GameTables
            .AsNoTracking()
            .Where(table => table.Id == tableId && table.UserIds.Contains(playerId))
            .Select(table => table.SheetIds)
            .FirstOrDefaultAsync();

        if (table is null || table.Count == 0)
            return [];

        return await db.Sheets
            .AsNoTracking()
            .Where(sheet => table.Contains(sheet.Id) && sheet.OwnerId == playerId)
            .Select(sheet => new SimpleSheetDto(sheet.Id, sheet.Name))
            .ToListAsync();
    }
}
