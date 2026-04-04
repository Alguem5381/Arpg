using Arpg.Application.Auth;
using Arpg.Application.Mapper;
using Arpg.Application.Queries;
using Arpg.Application.Repositories;
using Arpg.Contracts.Dto.Sheet;
using Arpg.Primitives.Codes;
using Arpg.Primitives.Constants;
using Arpg.Primitives.Results;
using FluentResults;

namespace Arpg.Application.Services;

public class SheetServices
    (
        IUnitOfWork unitOfWork,
        IUserContext userContext,
        ISheetQueries sheetQueries,
        ISheetRepository sheetRepository,
        ITemplateRepository templateRepository
    )
{
    private readonly SheetMapper _sheetMapper = new();
    public async Task<Result<Guid>> Create(SheetCreateDto dto)
    {
        var template = await templateRepository.GetTemplateById(dto.TemplateId);

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

    public async Task<Result<SheetDto>> GetAsync(Guid id)
    {
        var sheet = await sheetRepository.GetSheetReadOnlyAsync(id, userContext.Id);

        if (sheet == null)
            return Result.Fail(new NotFoundError("Sheet not found.")
                .WithMetadata(MetadataKey.Error, SheetCodes.SheetNotFound));

        var sheetDto = _sheetMapper.SheetToSheetDto(sheet);

        return Result.Ok(sheetDto);
    }

    public async Task<Result> ComputeDataAsync(ComputeSheetDto dto)
    {
        var sheet = await sheetRepository.GetSheetAsync(dto.Id, userContext.Id);

        if (sheet == null)
            return Result.Fail(new NotFoundError("Sheet not found.")
                .WithMetadata(MetadataKey.Error, SheetCodes.SheetNotFound));

        var template = await templateRepository.GetTemplateById(sheet.TemplateId);

        if (template == null)
            return Result.Fail(new ValidationError("Template not found.")
                .WithMetadata(MetadataKey.Error, TemplateCodes.TemplateNotFound));

        var result = sheet.ComputeData(dto.Data, template.Structure);

        if (result.IsFailed)
            return Result.Fail(result.Errors);

        await unitOfWork.CommitAsync();

        return Result.Ok();
    }

    public async Task<Result<SheetDto>> EditAsync(SheetEditDto dto)
    {
        var sheet = await sheetRepository.GetSheetAsync(dto.Id, userContext.Id);

        if (sheet == null)
            return Result.Fail(new NotFoundError("Sheet not found.")
                .WithMetadata(MetadataKey.Error, SheetCodes.SheetNotFound));

        sheet.Name = dto.Name;

        await unitOfWork.CommitAsync();

        var sheetDto = _sheetMapper.SheetToSheetDto(sheet);

        return Result.Ok(sheetDto);
    }

    public async Task<Result<List<SheetListDto>>> GetListAsync()
    {
        var sheets = await sheetQueries.GetSoftSheetListAsync(userContext.Id);
        return Result.Ok(sheets);
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