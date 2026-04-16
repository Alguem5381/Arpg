using Arpg.Contracts.Dto.Sheet;

namespace Arpg.Application.Queries;

public interface ISheetQueries
{
    Task<SheetDto?> GetSheetDtoAsync(Guid id, Guid ownerId);
    Task<List<SimpleSheetDto>> GetSimpleListAsync(Guid ownerId);
    Task<bool> AnyByTemplate(Guid templateId);
}