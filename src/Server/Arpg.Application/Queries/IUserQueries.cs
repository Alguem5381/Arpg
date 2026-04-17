using Arpg.Contracts.Dto.User;

namespace Arpg.Application.Queries;

public interface IUserQueries
{
    Task<UserInformationDto?> GetSimpleAsync(string username);
    Task<UserDto?> GetSelfAsync(Guid userId);
}