using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arpg.Client.Abstractions;
using Arpg.Contracts.Dto.Sheet;
using FluentResults;

namespace Arpg.Tests.Mocks;

public class MockSheetServices : ISheetServices
{
    public Task<Result<List<SimpleSheetDto>>> GetListAsync()
    {
        var list = new List<SimpleSheetDto>
        {
            new SimpleSheetDto(Guid.NewGuid(), "Guerreiro Nível 5"),
            new SimpleSheetDto(Guid.NewGuid(), "Mago Elfo Nível 3"),
            new SimpleSheetDto(Guid.NewGuid(), "Ladino Halfling Nível 4")
        };
        return Task.FromResult(Result.Ok(list));
    }
}
