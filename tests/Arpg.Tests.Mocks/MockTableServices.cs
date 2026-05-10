using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arpg.Client.Abstractions;
using Arpg.Contracts.Dto.GameTable;
using FluentResults;

namespace Arpg.Tests.Mocks;

public class MockTableServices : ITableServices
{
    public Task<Result<List<SimpleGameTableDto>>> GetListAsync()
    {
        var list = new List<SimpleGameTableDto>
        {
            new SimpleGameTableDto(Guid.NewGuid(), "Mesa do Dragão", Guid.NewGuid(), Guid.NewGuid(), DateTime.Now.AddDays(-10)),
            new SimpleGameTableDto(Guid.NewGuid(), "Aventuras no Espaço", Guid.NewGuid(), Guid.NewGuid(), DateTime.Now.AddDays(-5)),
            new SimpleGameTableDto(Guid.NewGuid(), "Sombras de Yharnam", Guid.NewGuid(), Guid.NewGuid(), DateTime.Now.AddDays(-2))
        };
        return Task.FromResult(Result.Ok(list));
    }
}
