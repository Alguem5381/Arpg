using Arpg.Contracts.Dto.User;
using Arpg.Core.Models.Customer;
using Riok.Mapperly.Abstractions;

namespace Arpg.Application.Mapper;

[Mapper]
public partial class UserMapper
{
    [MapProperty(nameof(NewDto.Username), nameof(User.DisplayName))]
    [MapperIgnoreSource(nameof(NewDto.Password))]
    [MapperIgnoreSource(nameof(NewDto.ConfirmPassword))]
    [MapperIgnoreSource(nameof(NewDto.Email))]
    public partial User NewDtoToUser(NewDto dto);

    [MapperIgnoreSource(nameof(User.Id))]
    public partial UserInformationDto UserToUserInformationDto(User user);

    [MapperIgnoreSource(nameof(User.Id))]
    public partial UserDto UserToUserDto(User user);
}