using Arpg.Core.Models.Customer;

namespace Arpg.Application.Repositories;

public interface ICodeRepository : IRepository<Code>
{
    Task<Code?> GetCode(Guid id);
}