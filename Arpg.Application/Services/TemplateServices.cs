using Arpg.Application.Abstractions;
using Arpg.Application.Auth;
using Arpg.Application.Queries;
using Arpg.Application.Repositories;
using Arpg.Application.Extensions;
using Arpg.Application.Mapper;
using Arpg.Contracts.Dto.Template;

using Arpg.Core.Models;

using Arpg.Shared.Constants;
using Arpg.Shared.Codes;
using Arpg.Shared.Results;

using FluentResults;
using FluentValidation;

using Arpg.Core.Interfaces.Security;

namespace Arpg.Application.Services;

public class TemplateServices
(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    IAccountRepository accountRepository,
    IPasswordHasher passwordHasher,
    ITemplateQueries templateQueries,
    ITemplateRepository templateRepository,
    IValidator<TemplateCreateDto> createDtoValidator,
    IValidator<TemplateEditDto> editDtoValidator,
    IValidator<TemplateDeleteDto> deleteDtoValidator
)
{
    private readonly TemplateMapper _templateMapper = new();
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

    public async Task<Result<TemplateDto>> GetAsync(Guid id)
    {
        var template = await templateRepository.GetTemplateById(id, userContext.Id);

        if (template == null)
            return Result.Fail(new NotFoundError("Template not found.")
                .WithMetadata(MetadataKey.Error, TemplateCodes.TemplateNotFound));
        
        var templateDto = _templateMapper.TemplateToTemplateDto(template);

        return Result.Ok(templateDto);
    }

    public async Task<Result<List<FlatTemplateDto>>> GetListAsync()
        => Result.Ok(await templateQueries.GetFlatTemplatesAsync(userContext.Id));

    public async Task<Result<Template>> EditAsync(TemplateEditDto dto)
    {
        var result = await editDtoValidator.ValidateAsync(dto);
        if (!result.IsValid)
            return result.ToResult();

        var template = await templateRepository.GetTemplateById(dto.Id, userContext.Id);

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

        var account = await accountRepository.GetByUserIdAsync(userContext.Id);

        if (account == null || !account.PasswordMatches(dto.Password, passwordHasher))
            return Result.Fail(new UnprocessableEntityError("Invalid password.")
                .WithMetadata(MetadataKey.Error, UserCodes.InvalidCredentials));

        var template = await templateRepository.TemplateExistsAsync(dto.Id, userContext.Id);

        switch (template)
        {
            case null:
                return Result.Fail(new NotFoundError("Template not found.")
                    .WithMetadata(MetadataKey.Error, TemplateCodes.TemplateNotFound));
            case false:
                await templateRepository.TemplateSoftDeleteAsync(dto.Id);
                break;
            default:
                await templateRepository.TemplateHardDeleteAsync(dto.Id);
                break;
        }

        return Result.Ok(true);
    }
}