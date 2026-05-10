using Arpg.Contracts.Dto.GameTable;
using Arpg.Contracts.Dto.Sheet;
using Arpg.Contracts.Dto.User;

namespace Arpg.Application.Queries;

public interface IGameTableQueries
{
    Task<List<SimpleGameTableDto>> GetAllAsync(Guid ownerId);
    Task<List<UserInformationDto>> GetUsersAsync(Guid tableId, Guid userId);
    Task<List<SimpleSheetDto>> GetAllSheetsAsync(Guid tableId, Guid dmId);
    Task<List<SimpleSheetDto>> GetUserSheetsAsync(Guid tableId, Guid playerId);
}