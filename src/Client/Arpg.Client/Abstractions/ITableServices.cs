using System.Collections.Generic;
using System.Threading.Tasks;
using Arpg.Contracts.Dto.GameTable;
using FluentResults;

namespace Arpg.Client.Abstractions;

public interface ITableServices
{
    Task<Result<List<SimpleGameTableDto>>> GetListAsync();
}
