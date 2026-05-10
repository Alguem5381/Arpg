using Arpg.Contracts.Dto.User;
using Arpg.Core.Models.Customer;
using Riok.Mapperly.Abstractions;

namespace Arpg.Application.Mapper;

[Mapper]
public partial class UserMapper
{
    [MapProperty(nameof(NewUserDto.Username), nameof(User.DisplayName))]
    [MapperIgnoreSource(nameof(NewUserDto.Password))]
    [MapperIgnoreSource(nameof(NewUserDto.ConfirmPassword))]
    [MapperIgnoreSource(nameof(NewUserDto.Email))]
    public partial User NewDtoToUser(NewUserDto dto);

    [MapperIgnoreSource(nameof(User.Id))]
    public partial UserInformationDto UserToUserInformationDto(User user);

    [MapperIgnoreSource(nameof(User.Id))]
    public partial UserDto UserToUserDto(User user);
}