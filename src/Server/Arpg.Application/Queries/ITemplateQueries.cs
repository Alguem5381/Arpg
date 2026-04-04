using Arpg.Contracts.Dto.Template;

namespace Arpg.Application.Queries;

public interface ITemplateQueries
{
    Task<List<FlatTemplateDto>> GetFlatTemplatesAsync(Guid ownerId);
}