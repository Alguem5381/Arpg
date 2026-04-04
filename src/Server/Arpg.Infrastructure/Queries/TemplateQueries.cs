using Arpg.Application.Mapper;
using Arpg.Application.Queries;
using Arpg.Contracts.Dto.Template;
using Arpg.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Arpg.Infrastructure.Queries;

public class TemplateQueries(AppDbContext db) : ITemplateQueries
{
    private readonly TemplateMapper _templateMapper = new();
    public async Task<List<FlatTemplateDto>> GetFlatTemplatesAsync(Guid ownerId)
        => await db.Templates
            .AsNoTracking()
            .Where(t => t.OwnerId == ownerId && !t.IsArchived)
            .Select(t => new FlatTemplateDto(t.Id,  t.Name, t.Description, t.CreationTime))
            .ToListAsync();
}