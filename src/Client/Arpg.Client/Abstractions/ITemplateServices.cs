using System.Collections.Generic;
using System.Threading.Tasks;
using Arpg.Contracts.Dto.Template;
using FluentResults;

namespace Arpg.Client.Abstractions;

public interface ITemplateServices
{
    Task<Result<List<SimpleTemplateDto>>> GetListAsync();
    Task<Result<SuccessCreateDto>> CreateAsync(NewTemplateDto dto);
    Task<Result> DeleteAsync(DeleteTemplateDto dto);
}
