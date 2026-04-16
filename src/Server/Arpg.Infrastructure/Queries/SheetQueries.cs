using Arpg.Application.Mapper;
using Arpg.Application.Queries;
using Arpg.Contracts.Dto.Sheet;
using Arpg.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Arpg.Infrastructure.Queries;

public class SheetQueries(AppDbContext db) : ISheetQueries
{
    private readonly AppDbContext _db = db;
    private readonly SheetMapper _sheetMapper = new();

    public async Task<List<SimpleSheetDto>> GetSimpleListAsync(Guid ownerId)
        => await _db.Sheets
            .AsNoTracking()
            .Where(s => s.OwnerId == ownerId)
            .Select(s => new SimpleSheetDto
            (
                s.Id,
                s.Name
            ))
            .ToListAsync();

    public async Task<bool> AnyByTemplate(Guid templateId)
        => await _db.Sheets
            .Where(s => s.TemplateId == templateId)
            .AnyAsync();

    public async Task<SheetDto?> GetSheetDtoAsync(Guid id, Guid ownerId)
    {
        var sheet = await _db.Sheets
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id && s.OwnerId == ownerId);

        return sheet == null ? null : _sheetMapper.SheetToSheetDto(sheet);
    }
}