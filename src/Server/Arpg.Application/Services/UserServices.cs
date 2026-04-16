using Arpg.Application.Auth;
using Arpg.Application.Extensions;
using Arpg.Application.Mapper;
using Arpg.Application.Repositories;
using Arpg.Contracts.Dto.User;
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
    IUserContext userContext,
    IValidator<EditDto> editDtoValidator
)
{
    private readonly UserMapper _userMapper = new();

    public async Task<Result<UserDto>> EditAsync(EditDto dto)
    {
        var validation = await editDtoValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return validation.ToResult();

        var user = await userRepository.GetAsync(userContext.Id);

        if (user == null)
            return Result.Fail(new NotFoundError("User not found.")
                .WithMetadata(MetadataKey.Error, UserCodes.UserNotFound));

        user.DisplayName = dto.DisplayName;

        await unitOfWork.CommitAsync();

        return Result.Ok(_userMapper.UserToUserDto(user));
    }
}