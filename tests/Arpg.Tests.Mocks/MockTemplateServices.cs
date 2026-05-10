using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arpg.Client.Abstractions;
using Arpg.Contracts.Dto.Template;
using FluentResults;

namespace Arpg.Tests.Mocks;

public class MockTemplateServices : ITemplateServices
{
    public Task<Result<List<SimpleTemplateDto>>> GetListAsync()
    {
        var list = new List<SimpleTemplateDto>
        {
            new SimpleTemplateDto(Guid.NewGuid(), "D&D 5e Padrão", "Template para aventuras épicas", DateTime.Now.AddMonths(-1)),
            new SimpleTemplateDto(Guid.NewGuid(), "Vampiro: A Máscara", "Sistema d10 para horror pessoal", DateTime.Now.AddMonths(-2)),
            new SimpleTemplateDto(Guid.NewGuid(), "Call of Cthulhu", "Sistema de investigação e terror cósmico", DateTime.Now.AddMonths(-3))
        };
        return Task.FromResult(Result.Ok(list));
    }
}
