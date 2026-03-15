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
            .FirstOrDefaultAsync(s => s.Id == id && s.OwnerId == ownerId);
    
    public async Task<Sheet?> GetSheetAsync(Guid id, Guid ownerId)
        => await _db.Sheets
            .FirstOrDefaultAsync(s => s.Id == id && s.OwnerId == ownerId);
}