using Arpg.Core.Models;

namespace Arpg.Application.Repositories;

public interface ISheetRepository : IRepository<Sheet>
{
    Task<Sheet?> GetSheetReadOnlyAsync(Guid id, Guid ownerId);
    Task<Sheet?> GetSheetAsync(Guid id, Guid ownerId);
    Task<bool> AnyAll(IEnumerable<Guid> userIds);
}