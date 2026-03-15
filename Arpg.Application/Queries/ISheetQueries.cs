using Arpg.Application.Repositories;
using Arpg.Contracts.Dto.Sheet;
using Arpg.Core.Models;

namespace Arpg.Application.Queries;

public interface ISheetQueries : IRepository<Sheet>
{
    Task<List<SheetListDto>> GetSoftSheetListAsync(Guid ownerId);
}