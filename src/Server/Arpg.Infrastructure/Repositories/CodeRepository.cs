using Arpg.Application.Repositories;
using Arpg.Core.Models.Customer;
using Arpg.Infrastructure.Data;

namespace Arpg.Infrastructure.Repositories;

public class CodeRepository(AppDbContext db) : Repository<Code>(db), ICodeRepository
{
    private readonly AppDbContext _db = db;

    public async Task<Code?> GetCode(Guid id)
        => await _db.Codes.FindAsync(id);
}