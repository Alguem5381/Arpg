using Arpg.Application.Auth;
using Arpg.Application.Extensions;
using Arpg.Application.Repositories;
using Arpg.Contracts.Dto.User;
using Arpg.Core.Interfaces.Security;
using Arpg.Primitives.Codes;
using Arpg.Primitives.Constants;
using Arpg.Primitives.Results;
using FluentResults;
using FluentValidation;

namespace Arpg.Application.Services;

public class UserServices
(
    IUnitOfWork unitOfWork,
    IUserRepository userRepository,
    IAccountRepository accountRepository,
    IPasswordHasher passwordHasher,
    IUserContext userContext,
    IValidator<EditDto> editDtoValidator,
    IValidator<DeleteDto> deleteDtoValidator
)
{

    public async Task<Result<InformationDto>> GetFlatAsync(string username)
    {
        var account = await accountRepository.GetReadOnlyAsync(username);

        if (account == null)
            return Result.Fail(new NotFoundError("Username not found.")
                .WithMetadata(MetadataKey.Error, UserCodes.UserNotFound));

        var user = await userRepository.GetCurrentReadOnlyAsync(account.UserId);

        if (user == null)
            return Result.Fail(new NotFoundError("Username not found.")
                .WithMetadata(MetadataKey.Error, UserCodes.UserNotFound));

        return Result.Ok(new InformationDto(user.DisplayName, account.Username));
    }

    public async Task<Result<UserDto>> GetSelfAsync()
    {
        var user = await userRepository.GetCurrentReadOnlyAsync(userContext.Id);
        var account = await accountRepository.GetByUserIdAsync(userContext.Id);

        if (user == null || account == null)
            return Result.Fail(new NotFoundError("Cannot find your user.")
                .WithMetadata(MetadataKey.Error, UserCodes.UserNotFound));

        return Result.Ok(new UserDto(user.DisplayName, account.Username));
    }

    public async Task<Result<UserDto>> EditAsync(EditDto dto)
    {
        var validation = await editDtoValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return validation.ToResult();

        var user = await userRepository.GetCurrentAsync(userContext.Id);

        if (user is null)
            return Result.Fail(new UnprocessableEntityError("Invalid password.")
                .WithMetadata(MetadataKey.Error, UserCodes.InvalidCredentials));

        user.DisplayName = dto.DisplayName;

        await unitOfWork.CommitAsync();

        var account = await accountRepository.GetByUserIdAsync(userContext.Id);

        if (account == null)
            return Result.Fail(new UnprocessableEntityError("Account not found."));

        return Result.Ok(new UserDto(user.DisplayName, account.Username));
    }

    public async Task<Result> DeleteAsync(DeleteDto dto)
    {
        var validation = await deleteDtoValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return validation.ToResult();

        var account = await accountRepository.GetByUserIdAsync(userContext.Id);

        if (account == null || !account.PasswordMatches(dto.Password, passwordHasher))
            return Result.Fail(new UnprocessableEntityError("Invalid password.")
                .WithMetadata(MetadataKey.Error, UserCodes.InvalidCredentials));

        var user = await userRepository.GetCurrentAsync(userContext.Id);

        if (user != null)
        {
            userRepository.Delete(user);
            await unitOfWork.CommitAsync();
        }

        return Result.Ok();
    }
}