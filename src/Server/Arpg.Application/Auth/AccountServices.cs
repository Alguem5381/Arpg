using Arpg.Application.Abstractions;
using Arpg.Application.Mapper;
using Arpg.Application.Repositories;
using Arpg.Application.Services;
using Arpg.Contracts.Dto.User;
using Arpg.Core.Interfaces.Security;
using Arpg.Core.Models.Customer;
using Arpg.Primitives.Codes;
using Arpg.Primitives.Constants;
using Arpg.Primitives.Results;
using FluentValidation;

namespace Arpg.Application.Auth;

public class AccountServices(
    ITokenServices tokenServices,
    IEmailServices emailServices,
    IUserContext userContext,
    IUserRepository userRepository,
    ICodeRepository codeRepository,
    IAccountRepository accountRepository,
    IPasswordHasher passwordHasher,
    IValidator<NewUserDto> newDtoValidator,
    IValidator<LoginDto> loginDtoValidator,
    IValidator<ValidateCodeDto> validateCodeDtoValidator,
    IValidator<DeleteUserDto> deleteDtoValidator,
    IUnitOfWork unitOfWork
) : BaseService
{
    private readonly UserMapper _userMapper = new();
    private readonly AccountMapper _accountMapper = new();

    public async Task<Result<CodeDto>> NewAsync(NewUserDto dto)
    {
        var validation = Validate(newDtoValidator, dto);
        if (validation.IsFailed)
            return validation;

        if (await userRepository.AnyAsync(dto.Username))
            return Result.Fail(new ConflictError("Username already exists.")
                .WithMetadata(MetadataKey.Error, UserCodes.UserConflict));

        if (await accountRepository.AnyAsync(dto.Email))
            return Result.Fail(new ValidationError("Invalid email.")
                .WithMetadata(MetadataKey.Error, DataFormatCodes.InvalidEmail)
                .WithMetadata(MetadataKey.PropertyName, nameof(NewUserDto.Email))
                .WithMetadata("PropertyValue", dto.Email)
                .WithMetadata("PropertyPath", nameof(NewUserDto.Email)));

        var user = _userMapper.NewDtoToUser(dto);
        var account = new Account(user.Id, dto.Email);

        account.SetInitialPassword(dto.Password, passwordHasher);

        var code = GenerateCode(account);

        userRepository.Add(user);
        accountRepository.Add(account);

        await unitOfWork.CommitAsync();

        await emailServices.SendCodeVerificationEmailAsync(dto.Email, code.Value);

        return Result.Ok(_accountMapper.CodeToCodeDto(code));
    }

    public async Task<Result<CodeDto>> LoginAsync(LoginDto dto)
    {
        var validation = Validate(loginDtoValidator, dto);
        if (validation.IsFailed)
            return validation;

        var account = await accountRepository.GetAsync(dto.Username);

        if (account is null)
            return Result.Fail(new UnauthorizedError("Invalid credentials")
                .WithMetadata(MetadataKey.Error, UserCodes.InvalidCredentials));

        if (account.IsLockedOut())
            return Result.Fail(new UnauthorizedError("Account is temporarily locked out due to multiple failed login attempts.")
                    .WithMetadata(MetadataKey.Error, UserCodes.AccountLocked));

        if (!account.PasswordMatches(dto.Password, passwordHasher))
        {
            account.RecordFailedLogin();
            await unitOfWork.CommitAsync();
            return Result.Fail(new UnauthorizedError("Invalid credentials")
                .WithMetadata(MetadataKey.Error, UserCodes.InvalidCredentials));
        }

        account.ResetFailedLogins();

        var code = GenerateCode(account);

        await unitOfWork.CommitAsync();

        await emailServices.SendCodeVerificationEmailAsync(account.Email, code.Value);

        return Result.Ok(_accountMapper.CodeToCodeDto(code));
    }

    private Code GenerateCode(Account account)
    {
        var code = new Code
        {
            OwnerId = account.OwnerId
        };

        codeRepository.Add(code);

        return code;
    }

    public async Task<Result<string>> ValidateCode(ValidateCodeDto dto)
    {
        var validation = Validate(validateCodeDtoValidator, dto);
        if (validation.IsFailed)
            return validation;

        var code = await codeRepository.GetCode(dto.Key);

        if (code == null)
            return Result.Fail(new NotFoundError("Code not found")
                .WithMetadata(MetadataKey.Error, CodeCodes.CodeNotFound));

        if (code.Value != dto.Value || code.Key != dto.Key)
            return Result.Fail(new UnauthorizedError("Invalid code")
                .WithMetadata(MetadataKey.Error, CodeCodes.InvalidCode));

        var user = await userRepository.GetAsync(code.OwnerId);

        if (user == null)
            return Result.Fail(new UnprocessableEntityError("Cannot solve this code")
                .WithMetadata(MetadataKey.Error, GeneralCodes.NotFound));

        codeRepository.Delete(code);
        await unitOfWork.CommitAsync();

        return tokenServices.GenerateToken(user, user.Username);
    }

    public async Task<Result> DeleteAsync(DeleteUserDto dto)
    {
        var validation = Validate(deleteDtoValidator, dto);
        if (validation.IsFailed)
            return validation;

        var account = await accountRepository.GetOwnerAsync(userContext.Id);

        if (account == null || !account.PasswordMatches(dto.Password, passwordHasher))
            return Result.Fail(new UnprocessableEntityError("Invalid credentials.")
                .WithMetadata(MetadataKey.Error, UserCodes.InvalidCredentials));

        var user = await userRepository.GetAsync(userContext.Id);

        if (user == null)
            return Result.Fail(new NotFoundError("User not found.")
                .WithMetadata(MetadataKey.Error, UserCodes.UserNotFound));

        accountRepository.Delete(account);
        userRepository.Delete(user);
        await unitOfWork.CommitAsync();

        return Result.Ok();
    }
}