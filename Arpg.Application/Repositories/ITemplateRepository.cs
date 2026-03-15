using Arpg.Core.Models;

namespace Arpg.Application.Repositories;

public interface ITemplateRepository : IRepository<Template>
{
    Task<Template?> GetTemplateById(Guid id, Guid ownerId);
    Task<Template?> GetTemplateById(Guid id);
    Task<bool?> TemplateExistsAsync(Guid id, Guid ownerId);
    Task TemplateSoftDeleteAsync(Guid id);
    Task TemplateHardDeleteAsync(Guid id);
}