using Arpg.Application.Auth;
using Arpg.Application.Mapper;
using Arpg.Application.Repositories;
using Arpg.Contracts.Dto.Structure;
using Arpg.Primitives.Constants;
using Arpg.Primitives.Codes;
using Arpg.Primitives.Results;

namespace Arpg.Application.Services;

public class StructureServices(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    ITemplateRepository templateRepository
)
{
    private readonly StructureMapper _structureMapper = new();

    public async Task<Result> UpdateStructureAsync(BatchStructureDto batch)
    {
        var template = await templateRepository.GetAsync(batch.TemplateId, userContext.Id);

        if (template is null)
            return Result.Fail(new NotFoundError("Template not found.")
                .With(Key.Error, TemplateCodes.TemplateNotFound));

        var update = _structureMapper.BatchUpdateDtoToBatchUpdate(batch);

        var result = template.Structure.Update(update);

        if (result.IsFailed)
            return Result.Fail(result.Errors);

        await unitOfWork.CommitAsync();

        return Result.Ok();
    }
}