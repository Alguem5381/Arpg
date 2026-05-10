using System.Collections.Generic;
using System.Threading.Tasks;
using Arpg.Contracts.Dto.Sheet;
using FluentResults;

namespace Arpg.Client.Abstractions;

public interface ISheetServices
{
    Task<Result<List<SimpleSheetDto>>> GetListAsync();
}