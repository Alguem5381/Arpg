using Arpg.Application.Repositories;
using Arpg.Core.Models;
using Arpg.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Arpg.Infrastructure.Repositories;

public class TemplateRepository(AppDbContext db) : Repository<Template>(db), ITemplateRepository
{
    private readonly AppDbContext _db = db;

    public async Task<Template?> GetTemplateById(Guid id, Guid ownerId)
        => await _db.Templates
            .FirstOrDefaultAsync(t => 
                t.Id == id && 
                !t.IsArchived &&
                t.OwnerId == ownerId);
    
    public async Task<Template?> GetTemplateById(Guid id)
        => await _db.Templates
            .FirstOrDefaultAsync(t => 
                t.Id == id && 
                !t.IsArchived);

    public async Task<bool?> TemplateExistsAsync(Guid id, Guid ownerId)
        => await _db.Templates
            .Where(t => t.Id == id && t.OwnerId == ownerId && !t.IsArchived)
            .Select(t => false)
            .FirstOrDefaultAsync();
    
    public async Task TemplateSoftDeleteAsync(Guid id)
        => await _db.Templates
            .Where(t => t.Id == id)
            .ExecuteUpdateAsync(t => t
                .SetProperty(p => p.Name, "Deleted template")
                .SetProperty(p => p.Description, "This template has archived, template is owned by system now.")
                .SetProperty(p => p.OwnerId, (Guid?)null)
                .SetProperty(p => p.IsArchived, true));
    
    public async Task TemplateHardDeleteAsync(Guid id)
        => await _db.Templates
            .Where(t => t.Id == id)
            .ExecuteDeleteAsync();
}