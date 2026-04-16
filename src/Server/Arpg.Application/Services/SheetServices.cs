using Arpg.Application.Auth;
using Arpg.Application.Extensions;
using Arpg.Application.Mapper;
using Arpg.Application.Queries;
using Arpg.Application.Repositories;

using Arpg.Contracts.Dto.Sheet;

using Arpg.Primitives.Codes;
using Arpg.Primitives.Constants;
using Arpg.Primitives.Results;

using FluentResults;
using FluentValidation;

namespace Arpg.Application.Services;

public class SheetServices
(
    IUnitOfWork unitOfWork,

    IUserContext userContext,

    ISheetRepository sheetRepository,
    ITemplateRepository templateRepository,

    IValidator<CreateDto> createValidator,
    IValidator<EditDto> editValidator,
    IValidator<ComputeDto> computeValidator
)
{
    private readonly SheetMapper _sheetMapper = new();

    public async Task<Result<Guid>> CreateAsync(CreateDto dto)
    {
        var validation = await createValidator.ValidateAsync(dto);

        if (!validation.IsValid)
            return validation.ToResult();

        var template = await templateRepository.GetAsync(dto.TemplateId);

        if (template == null)
            return Result.Fail(new NotFoundError("Template not found.")
                .WithMetadata(MetadataKey.Error, TemplateCodes.TemplateNotFound));

        var sheet = _sheetMapper.SheetCreateDto(dto);

        var result = sheet.Populate(template.Structure);

        if (result.IsFailed)
            return Result.Fail(result.Errors);

        sheet.OwnerId = userContext.Id;

        sheetRepository.Add(sheet);
        await unitOfWork.CommitAsync();

        return Result.Ok(sheet.Id);
    }

    public async Task<Result> ComputeDataAsync(ComputeDto dto)
    {
        var validation = await computeValidator.ValidateAsync(dto);

        if (!validation.IsValid)
            return validation.ToResult();

        var sheet = await sheetRepository.GetSheetAsync(dto.Id, userContext.Id);

        if (sheet == null)
            return Result.Fail(new NotFoundError("Sheet not found.")
                .WithMetadata(MetadataKey.Error, SheetCodes.SheetNotFound));

        var template = await templateRepository.GetAsync(sheet.TemplateId);

        if (template == null)
            return Result.Fail(new ValidationError("Template not found.")
                .WithMetadata(MetadataKey.Error, TemplateCodes.TemplateNotFound));

        var result = sheet.ComputeData(dto.Data, template.Structure);

        if (result.IsFailed)
            return Result.Fail(result.Errors);

        await unitOfWork.CommitAsync();

        return Result.Ok();
    }

    public async Task<Result<SheetDto>> EditAsync(EditDto dto)
    {
        var validation = await editValidator.ValidateAsync(dto);

        if (!validation.IsValid)
            return validation.ToResult();

        var sheet = await sheetRepository.GetSheetAsync(dto.Id, userContext.Id);

        if (sheet == null)
            return Result.Fail(new NotFoundError("Sheet not found.")
                .WithMetadata(MetadataKey.Error, SheetCodes.SheetNotFound));

        sheet.Name = dto.Name;

        await unitOfWork.CommitAsync();

        return Result.Ok(_sheetMapper.SheetToSheetDto(sheet));
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var sheet = await sheetRepository.GetSheetAsync(id, userContext.Id);

        if (sheet == null)
            return Result.Fail(new NotFoundError("Sheet not found")
                .WithMetadata(MetadataKey.Error, SheetCodes.SheetNotFound));

        sheetRepository.Delete(sheet);
        await unitOfWork.CommitAsync();

        return Result.Ok();
    }
}