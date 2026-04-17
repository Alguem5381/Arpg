using Arpg.Core.Models;
using Arpg.Core.Models.Tabletop;

namespace Arpg.Application.Repositories;

public interface IGameTableRepository : IRepository<GameTable>
{
    Task<GameTable?> GetAsync(Guid id, Guid dmId);
}
