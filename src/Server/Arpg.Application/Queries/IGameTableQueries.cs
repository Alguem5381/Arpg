using Arpg.Core.Models;
using Arpg.Core.Models.Tabletop;

namespace Arpg.Application.Queries;

public interface IGameTableQueries
{
    Task<List<GameTable>> GetAllAsync(Guid dmId);
}
