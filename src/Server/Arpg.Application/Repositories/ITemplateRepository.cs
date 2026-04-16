using Arpg.Core.Models.Definitions;

namespace Arpg.Application.Repositories;

public interface ITemplateRepository : IRepository<Template>
{
    Task<Template?> GetAsync(Guid id, Guid ownerId);
    Task<Template?> GetAsync(Guid id);
}