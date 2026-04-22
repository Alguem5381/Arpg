using Arpg.Application.Auth;
using Arpg.Application.Mapper;
using Arpg.Application.Repositories;
using Arpg.Contracts.Dto.Sheet;
using Arpg.Primitives.Codes;
using Arpg.Primitives.Constants;
using Arpg.Primitives.Results;
using FluentValidation;

namespace Arpg.Application.Services;

public class SheetServices(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    ISheetRepository sheetRepository,
    ITemplateRepository templateRepository,
    IValidator<NewSheetDto> createValidator,
    IValidator<EditSheetDto> editValidator,
    IValidator<ComputeSheetDto> computeValidator
) : BaseService
{
    private readonly SheetMapper _sheetMapper = new();

    public async Task<Result<Guid>> CreateAsync(NewSheetDto dto)
    {
        var validation = Validate(createValidator, dto);
        if (validation.IsFailed)
            return validation;

        var template = await templateRepository.GetAsync(dto.TemplateId);

        if (template == null)
            return Result.Fail(new NotFoundError("Template not found.")
                .With(Key.Error, TemplateCodes.TemplateNotFound));

        var sheet = _sheetMapper.SheetCreateDto(dto);

        var result = sheet.Populate(template.Structure);

        if (result.IsFailed)
            return Result.Fail(result.Errors);

        sheet.OwnerId = userContext.Id;

        sheetRepository.Add(sheet);
        await unitOfWork.CommitAsync();

        return Result.Ok(sheet.Id);
    }

    public async Task<Result> ComputeDataAsync(ComputeSheetDto dto)
    {
        var validation = Validate(computeValidator, dto);
        if (validation.IsFailed)
            return validation;

        var sheet = await sheetRepository.GetSheetAsync(dto.Id, userContext.Id);

        if (sheet == null)
            return Result.Fail(new NotFoundError("Sheet not found.")
                .With(Key.Error, SheetCodes.SheetNotFound));

        var template = await templateRepository.GetAsync(sheet.TemplateId);

        if (template == null)
            return Result.Fail(new ValidationError("Template not found.")
                .With(Key.Error, TemplateCodes.TemplateNotFound));

        var result = sheet.ComputeData(dto.Data, template.Structure);

        if (result.IsFailed)
            return Result.Fail(result.Errors);

        await unitOfWork.CommitAsync();

        return Result.Ok();
    }

    public async Task<Result<SheetDto>> EditAsync(EditSheetDto dto)
    {
        var validation = Validate(editValidator, dto);
        if (validation.IsFailed)
            return validation;

        var sheet = await sheetRepository.GetSheetAsync(dto.Id, userContext.Id);

        if (sheet == null)
            return Result.Fail(new NotFoundError("Sheet not found.")
                .With(Key.Error, SheetCodes.SheetNotFound));

        sheet.Name = dto.Name;

        await unitOfWork.CommitAsync();

        return Result.Ok(_sheetMapper.SheetToSheetDto(sheet));
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var sheet = await sheetRepository.GetSheetAsync(id, userContext.Id);

        if (sheet == null)
            return Result.Fail(new NotFoundError("Sheet not found")
                .With(Key.Error, SheetCodes.SheetNotFound));

        sheetRepository.Delete(sheet);
        await unitOfWork.CommitAsync();

        return Result.Ok();
    }
}