using Arpg.Core.Models.Customer;

using Arpg.Application.Extensions;
using Arpg.Application.Mapper;
using Arpg.Application.Repositories;
using Arpg.Core.Interfaces.Security;
using Arpg.Application.Abstractions;

using Arpg.Contracts.Dto.General;
using Arpg.Contracts.Dto.User;

using Arpg.Shared.Codes;
using Arpg.Shared.Constants;
using Arpg.Shared.Results;

using FluentResults;
using FluentValidation;

namespace Arpg.Application.Auth;

public class AccountServices
    (
        ITokenServices tokenServices, 
        IUserRepository userRepository, 
        ICodeRepository codeRepository, 
        IAccountRepository accountRepository,
        IPasswordHasher passwordHasher,
        IValidator<NewDto> newDtoValidator,
        IValidator<LoginDto> loginDtoValidator,
        IEmailServices emailServices,
        IUnitOfWork unitOfWork
    )
{
    private readonly UserMapper _userMapper = new();
    private readonly AccountMapper _accountMapper = new();
    
    public async Task<Result<CodeDto>> NewAsync(NewDto dto)
    {
        var validation = await newDtoValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return validation.ToResult();

        if (await accountRepository.AnyAsync(dto.Username))
            return Result.Fail(new ConflictError("Username already exists.")
                .WithMetadata(MetadataKey.Error, UserCodes.UserConflict));

        if (await accountRepository.AnyAsync(dto.Email))
            return Result.Fail(new ConflictError("Cannot create, please, see information again.")
                .WithMetadata(MetadataKey.Error, DataFormatCodes.InvalidEmailFormat));

        var user = _userMapper.NewDtoToUser(dto);
        var account = new Account(user.Id);
        
        account.SetInitialPassword(dto.Password, passwordHasher);

        userRepository.Add(user);
        accountRepository.Add(account);
        
        var code = GenerateCode(account);
        await emailServices.SendCodeVerificationEmailAsync(account.Email, code.Value.ToString());
        
        await unitOfWork.CommitAsync();

        return Result.Ok(_accountMapper.CodeToCodeDto(code));
    }

    public async Task<Result<CodeDto>> LoginAsync(LoginDto dto)
    {
        var validation = await loginDtoValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return validation.ToResult();

        var account = await accountRepository.GetReadOnlyAsync(dto.Username);
        
        if (account is null)
            return Result.Fail(new UnauthorizedError("Invalid credentials")
                .WithMetadata(MetadataKey.Error, UserCodes.InvalidCredentials));

        if (account.IsLockedOut())
            return Result.Fail(new UnauthorizedError("Account is temporarily locked out due to multiple failed login attempts.")
                .WithMetadata(MetadataKey.Error, UserCodes.InvalidCredentials));

        if (!account.PasswordMatches(dto.Password, passwordHasher))
        {
            account.RecordFailedLogin();
            await unitOfWork.CommitAsync();
            return Result.Fail(new UnauthorizedError("Invalid credentials")
                .WithMetadata(MetadataKey.Error, UserCodes.InvalidCredentials));
        }

        account.ResetFailedLogins();

        var user = await userRepository.GetAsync(account.UserId);

        if (user is null)
            return Result.Fail(new UnauthorizedError("Invalid credentials")
                .WithMetadata(MetadataKey.Error, UserCodes.InvalidCredentials));
        
        var code = GenerateCode(account);
        await emailServices.SendCodeVerificationEmailAsync(account.Email, code.Value.ToString());
        
        await unitOfWork.CommitAsync();

        return Result.Ok(_accountMapper.CodeToCodeDto(code));
    }
    
    private Code GenerateCode(Account account)
    {
        var code = new Code
        {
            OwnerId = account.Id
        };
        
        codeRepository.Add(code);
        
        return code;
    }

    public async Task<Result<string>> ValidateCode(Guid id, int value)
    {
        var code = await codeRepository.GetCode(id);

        if (code == null || code.Value != value)
            return Result.Fail(new UnprocessableEntityError("Cannot solve this code")
                .WithMetadata(MetadataKey.Error, GeneralCodes.NotFound));

        var account = await accountRepository.GetAsync(code.OwnerId);

        if (account == null)
            return Result.Fail(new UnprocessableEntityError("Cannot solve this code")
                .WithMetadata(MetadataKey.Error, GeneralCodes.NotFound));

        var user = await userRepository.GetAsync(account.UserId);

        if (user == null)
            return  Result.Fail(new UnprocessableEntityError("Cannot solve this code")
                .WithMetadata(MetadataKey.Error, GeneralCodes.NotFound));

        codeRepository.Delete(code);
        await unitOfWork.CommitAsync();

        return tokenServices.GenerateToken(user, account.Username);
    }
}