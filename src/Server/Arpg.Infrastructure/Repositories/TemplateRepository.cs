using Arpg.Application.Repositories;
using Arpg.Core.Models.Definitions;
using Arpg.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Arpg.Infrastructure.Repositories;

public class TemplateRepository(AppDbContext db) : Repository<Template>(db), ITemplateRepository
{
    private readonly AppDbContext _db = db;

    public async Task<Template?> GetAsync(Guid id, Guid ownerId)
        => await _db.Templates
            .FirstOrDefaultAsync(t =>
                t.Id == id &&
                !t.IsArchived &&
                t.OwnerId == ownerId);

    public async Task<Template?> GetAsync(Guid id)
        => await _db.Templates
            .FirstOrDefaultAsync(t =>
                t.Id == id &&
                !t.IsArchived);
}