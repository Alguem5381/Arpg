using Arpg.Application.Auth;
using Arpg.Application.Mapper;
using Arpg.Application.Repositories;
using Arpg.Contracts.Dto.User;
using Arpg.Primitives.Codes;
using Arpg.Primitives.Constants;
using Arpg.Primitives.Results;
using FluentValidation;

namespace Arpg.Application.Services;

public class UserServices(
    IUnitOfWork unitOfWork,
    IUserRepository userRepository,
    IUserContext userContext,
    IValidator<EditUserDto> editDtoValidator
) : BaseService
{
    private readonly UserMapper _userMapper = new();

    public async Task<Result<UserDto>> EditAsync(EditUserDto dto)
    {
        var validation = Validate(editDtoValidator, dto);
        if (validation.IsFailed)
            return validation;

        var user = await userRepository.GetAsync(userContext.Id);

        if (user == null)
            return Result.Fail(new NotFoundError("User not found.")
                .With(Key.Error, UserCodes.UserNotFound));

        user.DisplayName = dto.DisplayName;

        await unitOfWork.CommitAsync();

        return Result.Ok(_userMapper.UserToUserDto(user));
    }
}