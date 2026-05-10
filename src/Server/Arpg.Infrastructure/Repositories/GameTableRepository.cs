using Arpg.Application.Repositories;
using Arpg.Core.Models.Tabletop;
using Arpg.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Arpg.Infrastructure.Repositories;

public class GameTableRepository(AppDbContext db) : Repository<GameTable>(db), IGameTableRepository
{
    private readonly AppDbContext _db = db;

    public async Task<GameTable?> GetAsync(Guid id, Guid dmId)
        => await _db.GameTables.FirstOrDefaultAsync(x => x.Id == id && x.DmId == dmId);
}
