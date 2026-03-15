using Arpg.Application.Queries;
using Arpg.Contracts.Dto.Sheet;
using Arpg.Core.Models;
using Arpg.Infrastructure.Data;
using Arpg.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Arpg.Infrastructure.Queries;

public class SheetQueries(AppDbContext db) : Repository<Sheet>(db), ISheetQueries
{
    private readonly AppDbContext _db = db;

    public async Task<List<SheetListDto>> GetSoftSheetListAsync(Guid ownerId)
        => await _db.Sheets
            .AsNoTracking()
            .Where(s => s.OwnerId == ownerId)
            .Select(s => new SheetListDto
            (
                s.Id,
                s.Name
            ))
            .ToListAsync();
}