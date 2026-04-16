using Arpg.Contracts.Dto.Template;

namespace Arpg.Application.Queries;

public interface ITemplateQueries
{
    Task<List<SimpleTemplateDto>> GetListAsync(Guid ownerId);
    Task<TemplateDto?> GetTemplate(Guid id, Guid ownerId);
}