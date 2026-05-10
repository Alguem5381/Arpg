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
            return new ConflictError("Username already exists.")
                .With(Key.Error, UserCodes.UserConflict);

        if (await accountRepository.AnyAsync(dto.Email))
            return new ValidationError("Invalid email.")
                .With(Key.Error, DataFormatCodes.InvalidEmail)
                .With(Key.PropertyName, nameof(NewUserDto.Email))
                .With("PropertyValue", dto.Email)
                .With("PropertyPath", nameof(NewUserDto.Email));

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

        var user = await userRepository.GetAsync(dto.Username);

        if (user is null)
            return new UnauthorizedError("Invalid credentials")
                .With(Key.Error, UserCodes.InvalidCredentials);

        var account = await accountRepository.GetByOwnerAsync(user.Id);

        if (account is null)
            return new UnauthorizedError("Invalid credentials")
                .With(Key.Error, UserCodes.InvalidCredentials);

        if (account.IsLockedOut())
            return new UnauthorizedError("Account is temporarily locked out due to multiple failed login attempts.")
                .With(Key.Error, UserCodes.AccountLocked);

        if (!account.PasswordMatches(dto.Password, passwordHasher))
        {
            account.RecordFailedLogin();
            await unitOfWork.CommitAsync();
            return new UnauthorizedError("Invalid credentials")
                .With(Key.Error, UserCodes.InvalidCredentials);
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
            return new NotFoundError("Code not found")
                .With(Key.Error, CodeCodes.CodeNotFound);

        if (code.Value != dto.Value || code.Key != dto.Key)
            return new UnauthorizedError("Invalid code")
                .With(Key.Error, CodeCodes.InvalidCode);

        var user = await userRepository.GetAsync(code.OwnerId);
        var account = await accountRepository.GetByOwnerAsync(code.OwnerId);

        if (user == null || account == null)
            return new UnprocessableError("Cannot solve this code")
                .With(Key.Error, GeneralCodes.NotFound);

        account.IsValid = true;

        codeRepository.Delete(code);
        await unitOfWork.CommitAsync();

        return tokenServices.GenerateToken(user, user.Username);
    }

    public async Task<Result> DeleteAsync(DeleteUserDto dto)
    {
        var validation = Validate(deleteDtoValidator, dto);
        if (validation.IsFailed)
            return validation;

        var account = await accountRepository.GetByOwnerAsync(userContext.Id);

        if (account == null || !account.PasswordMatches(dto.Password, passwordHasher))
            return new UnprocessableError("Invalid credentials.")
                .With(Key.Error, UserCodes.InvalidCredentials);

        var user = await userRepository.GetAsync(userContext.Id);

        if (user == null)
            return new NotFoundError("User not found.")
                .With(Key.Error, UserCodes.UserNotFound);

        accountRepository.Delete(account);
        userRepository.Delete(user);
        await unitOfWork.CommitAsync();

        return Result.Ok();
    }
}