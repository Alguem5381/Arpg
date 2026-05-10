using Arpg.Application.Auth;
using Arpg.Application.Queries;
using Arpg.Application.Repositories;
using Arpg.Contracts.Dto.Template;
using Arpg.Core.Interfaces.Security;
using Arpg.Core.Models.Definitions;
using Arpg.Primitives.Codes;
using Arpg.Primitives.Constants;
using Arpg.Primitives.Results;
using FluentValidation;

namespace Arpg.Application.Services;

public class TemplateServices(
    IUserContext userContext,
    IPasswordHasher passwordHasher,
    IAccountRepository accountRepository,
    ITemplateRepository templateRepository,
    ISheetQueries sheetRepository,
    IUnitOfWork unitOfWork,
    IValidator<NewTemplateDto> createDtoValidator,
    IValidator<EditTemplateDto> editDtoValidator,
    IValidator<DeleteTemplateDto> deleteDtoValidator
) : BaseService
{
    public async Task<Result<Guid>> CreateAsync(NewTemplateDto dto)
    {
        var validation = Validate(createDtoValidator, dto);
        if (validation.IsFailed)
            return validation;

        var template = new Template()
        {
            Name = dto.Name,
            OwnerId = userContext.Id,
        };

        templateRepository.Add(template);
        await unitOfWork.CommitAsync();

        return Result.Ok(template.Id);
    }

    public async Task<Result<Template>> EditAsync(EditTemplateDto dto)
    {
        var validation = Validate(editDtoValidator, dto);
        if (validation.IsFailed)
            return validation;

        var template = await templateRepository.GetAsync(dto.Id, userContext.Id);

        if (template == null)
            return Result.Fail(new NotFoundError("Template not found.")
                .With(Key.Error, TemplateCodes.TemplateNotFound));

        template.Name = dto.Name;
        template.Description = dto.Description;

        await unitOfWork.CommitAsync();

        return Result.Ok(template);
    }

    public async Task<Result<bool>> DeleteAsync(DeleteTemplateDto dto)
    {
        var validation = Validate(deleteDtoValidator, dto);
        if (validation.IsFailed)
            return validation;

        var account = await accountRepository.GetByOwnerAsync(userContext.Id);

        if (account == null || !account.PasswordMatches(dto.Password, passwordHasher))
            return Result.Fail(new UnprocessableError("Invalid password.")
                .With(Key.Error, UserCodes.InvalidCredentials));

        var template = await templateRepository.GetAsync(dto.Id, userContext.Id);

        if (template == null)
            return Result.Fail(new NotFoundError("Template not found.")
                .With(Key.Error, TemplateCodes.TemplateNotFound));

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