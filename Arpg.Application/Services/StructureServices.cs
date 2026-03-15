using Arpg.Application.Abstractions;
using Arpg.Application.Auth;
using Arpg.Application.Mapper;
using Arpg.Application.Repositories;

using Arpg.Contracts.Dto.Structure;

using Arpg.Shared.Constants;
using Arpg.Shared.Codes;
using Arpg.Shared.Results;

using FluentResults;

namespace Arpg.Application.Services;

public class StructureServices(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    ITemplateRepository templateRepository)
{
    private readonly StructureMapper _structureMapper = new();
    public async Task<Result> UpdateStructureAsync(BatchUpdateDto batch)
    {
        var template = await templateRepository.GetTemplateById(batch.TemplateId, userContext.Id);

        if (template is null)
            return Result.Fail(new NotFoundError("Template not found.")
                .WithMetadata(MetadataKey.Error, TemplateCodes.TemplateNotFound));

        var update = _structureMapper.BatchUpdateDtoToBatchUpdate(batch);

        var result = template.Structure.Update(update);

        if (result.IsFailed)
            return Result.Fail(result.Errors);

        await unitOfWork.CommitAsync();

        return Result.Ok();
    }
}