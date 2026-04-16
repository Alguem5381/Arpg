using Arpg.Application.Mapper;
using Arpg.Application.Queries;
using Arpg.Contracts.Dto.Template;
using Arpg.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Arpg.Infrastructure.Queries;

public class TemplateQueries(AppDbContext db) : ITemplateQueries
{
    private readonly TemplateMapper _templateMapper = new();

    public async Task<List<SimpleTemplateDto>> GetListAsync(Guid ownerId)
        => await db.Templates
            .AsNoTracking()
            .Where(t => t.OwnerId == ownerId && !t.IsArchived)
            .Select(t => new SimpleTemplateDto(t.Id, t.Name, t.Description, t.CreationTime))
            .ToListAsync();

    public async Task<TemplateDto?> GetTemplate(Guid id, Guid ownerId)
    {
        var template = await db.Templates
            .AsNoTracking()
            .Where(t => t.Id == id && t.OwnerId == ownerId)
            .FirstOrDefaultAsync();

        return template == null ? null : _templateMapper.TemplateToTemplateDto(template);
    }
}