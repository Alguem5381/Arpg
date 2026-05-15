using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arpg.Contracts.Dto.Sheet;
using FluentResults;

namespace Arpg.Client.Abstractions;

public interface ISheetServices
{
    Task<Result<List<SimpleSheetDto>>> GetListAsync();
    Task<Result<SheetDto>> GetAsync(Guid id);
    Task<Result> CreateAsync(NewSheetDto dto);
    Task<Result> EditAsync(EditSheetDto dto);
    Task<Result> ComputeDataAsync(ComputeSheetDto dto);
    Task<Result> DeleteAsync(Guid id);
}