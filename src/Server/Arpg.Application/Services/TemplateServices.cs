using Arpg.Application.Auth;
using Arpg.Application.Extensions;
using Arpg.Application.Mapper;
using Arpg.Application.Queries;
using Arpg.Application.Repositories;
using Arpg.Contracts.Dto.Template;
using Arpg.Core.Interfaces.Security;
using Arpg.Core.Models.Definitions;
using Arpg.Primitives.Codes;
using Arpg.Primitives.Constants;
using Arpg.Primitives.Results;
using FluentResults;
using FluentValidation;

namespace Arpg.Application.Services;

public class TemplateServices
(
    IUserContext userContext,
    IPasswordHasher passwordHasher,

    IAccountRepository accountRepository,
    ITemplateRepository templateRepository,
    ISheetQueries sheetRepository,

    IUnitOfWork unitOfWork,

    IValidator<TemplateCreateDto> createDtoValidator,
    IValidator<TemplateEditDto> editDtoValidator,
    IValidator<TemplateDeleteDto> deleteDtoValidator
)
{
    public async Task<Result<Guid>> CreateAsync(TemplateCreateDto dto)
    {
        var result = await createDtoValidator.ValidateAsync(dto);
        if (!result.IsValid)
            return result.ToResult();

        var template = new Template()
        {
            Name = dto.Name,
            OwnerId = userContext.Id,
        };

        templateRepository.Add(template);
        await unitOfWork.CommitAsync();

        return Result.Ok(template.Id);
    }

    public async Task<Result<Template>> EditAsync(TemplateEditDto dto)
    {
        var result = await editDtoValidator.ValidateAsync(dto);
        if (!result.IsValid)
            return result.ToResult();

        var template = await templateRepository.GetAsync(dto.Id, userContext.Id);

        if (template == null)
            return Result.Fail(new NotFoundError("Template not found.")
                .WithMetadata(MetadataKey.Error, TemplateCodes.TemplateNotFound));

        template.Name = dto.Name;
        template.Description = dto.Description;

        await unitOfWork.CommitAsync();

        return Result.Ok(template);
    }

    public async Task<Result<bool>> DeleteAsync(TemplateDeleteDto dto)
    {
        var result = await deleteDtoValidator.ValidateAsync(dto);
        if (!result.IsValid)
            return result.ToResult();

        var account = await accountRepository.GetOwnerAsync(userContext.Id);

        if (account == null || !account.PasswordMatches(dto.Password, passwordHasher))
            return Result.Fail(new UnprocessableEntityError("Invalid password.")
                .WithMetadata(MetadataKey.Error, UserCodes.InvalidCredentials));

        var template = await templateRepository.GetAsync(dto.Id, userContext.Id);

        if (template == null)
            return Result.Fail(new NotFoundError("Template not found.")
                .WithMetadata(MetadataKey.Error, TemplateCodes.TemplateNotFound));

        var hasSheets = await sheetRepository.AnyByTemplate(dto.Id);

        if (hasSheets)
        {
            template.Name = "Deleted template";
            template.Description = "This template has archived, template is owned by system now.";
            template.OwnerId = Guid.Empty;
            template.IsArchived = true;
        }
        else
            templateRepository.Delete(template);

        await unitOfWork.CommitAsync();

        return Result.Ok(true);
    }
}