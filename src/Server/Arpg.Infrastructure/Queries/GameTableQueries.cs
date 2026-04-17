using Arpg.Application.Queries;
using Arpg.Core.Models;
using Arpg.Core.Models.Tabletop;
using Arpg.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Arpg.Infrastructure.Queries;

public class GameTableQueries(AppDbContext db) : IGameTableQueries
{
    public async Task<List<GameTable>> GetAllAsync(Guid dmId)
        => await db.GameTables.Where(x => x.DmId == dmId).ToListAsync();
}
