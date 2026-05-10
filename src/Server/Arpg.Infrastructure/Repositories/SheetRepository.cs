using Arpg.Application.Repositories;
using Arpg.Core.Models;
using Arpg.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Arpg.Infrastructure.Repositories;

public class SheetRepository(AppDbContext db) : Repository<Sheet>(db), ISheetRepository
{
    private readonly AppDbContext _db = db;

    public async Task<Sheet?> GetSheetReadOnlyAsync(Guid id, Guid ownerId)
        => await _db.Sheets
            .AsNoTracking()
            .FirstOrDefaultAsync(sheet => sheet.Id == id && sheet.OwnerId == ownerId);

    public async Task<Sheet?> GetSheetAsync(Guid id, Guid ownerId)
        => await _db.Sheets
            .FirstOrDefaultAsync(sheet => sheet.Id == id && sheet.OwnerId == ownerId);

    public async Task<bool> AnyAll(IEnumerable<Guid> sheetsId)
        => await _db.Sheets.CountAsync(sheet => sheetsId.Contains(sheet.Id)) == sheetsId.Count();

    public async Task<List<Guid>> FilterValidByOwnersAsync(IEnumerable<Guid> sheetIds, IEnumerable<Guid> validOwnerIds)
        => await _db.Sheets
            .Where(s => sheetIds.Contains(s.Id) && validOwnerIds.Contains(s.OwnerId))
            .Select(s => s.Id)
            .ToListAsync();
}