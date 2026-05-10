using Arpg.Application.Auth;
using Arpg.Application.Mapper;
using Arpg.Application.Repositories;
using Arpg.Contracts.Dto.GameTable;
using Arpg.Primitives.Codes;
using Arpg.Primitives.Constants;
using Arpg.Primitives.Enums.Template;
using Arpg.Primitives.Results;
using FluentValidation;

namespace Arpg.Application.Services;

public class GameTableServices(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    IGameTableRepository gameTableRepository,
    IUserRepository userRepository,
    ISheetRepository sheetRepository,
    IValidator<NewTableDto> newDtoValidator,
    IValidator<GameTableOperationDto> batchPlayersDtoValidator
) : BaseService
{
    private readonly GameTableMapper _gameTableMapper = new();

    public async Task<Result<GameTableDto>> CreateAsync(NewTableDto request)
    {
        var validation = Validate(newDtoValidator, request);
        if (validation.IsFailed)
            return validation;

        var gameTable = _gameTableMapper.ToGameTable(request);

        gameTable.SetGameMaster(userContext.Id);

        gameTableRepository.Add(gameTable);
        await unitOfWork.CommitAsync();

        return _gameTableMapper.ToGameTableDto(gameTable);
    }

    public async Task<Result> UpdateUsersAsync(GameTableBatchDto request)
    {
        var validation = Validate(batchPlayersDtoValidator, request.Batch);
        if (validation.IsFailed)
            return validation;

        var gameTable = await gameTableRepository.GetAsync(request.TableId, userContext.Id);

        if (gameTable is null)
            return new NotFoundError("Table not found")
                .With(Key.Error, GameTableCodes.TableNotFound);

        var domainBatch = request.Batch.Select(dto => _gameTableMapper.ToGameTableOperation(dto));

        var result = gameTable.UpdateUsers(domainBatch);
        if (result.IsFailed)
            return result;

        var validUserIds = await userRepository.FilterExistingAsync(gameTable.UserIds);

        var addedUserIds = request.Batch.Where(x => x.Operation == Operation.Add).Select(x => x.Id);
        var missingAddedUsers = addedUserIds.Except(validUserIds).ToList();

        if (missingAddedUsers.Count != 0)
            return new UnprocessableError("Some users do not exist in the system.")
                .With(Key.Error, GameTableCodes.PlayersNotExists);

        var validSheetIds = await sheetRepository.FilterValidByOwnersAsync(gameTable.SheetIds, validUserIds);

        gameTable.SyncUsers(validUserIds);
        gameTable.SyncSheets(validSheetIds);

        await unitOfWork.CommitAsync();

        return Result.Ok();
    }

    public async Task<Result> UpdateSheetsAsync(GameTableBatchDto request)
    {
        var validation = Validate(batchPlayersDtoValidator, request.Batch);
        if (validation.IsFailed)
            return validation;

        var gameTable = await gameTableRepository.GetAsync(request.TableId, userContext.Id);

        if (gameTable is null)
            return new NotFoundError("Table not found")
                .With(Key.Error, GameTableCodes.TableNotFound);

        var domainBatch = request.Batch.Select(dto => _gameTableMapper.ToGameTableOperation(dto));

        var result = gameTable.UpdateSheets(domainBatch);
        if (result.IsFailed)
            return result;

        var validSheetIds = await sheetRepository.FilterValidByOwnersAsync(gameTable.SheetIds, gameTable.UserIds);

        var addedSheetIds = request.Batch.Where(x => x.Operation == Operation.Add).Select(x => x.Id);
        var missingAddedSheets = addedSheetIds.Except(validSheetIds).ToList();

        if (missingAddedSheets.Count != 0)
            return new UnprocessableError("Some sheets do not exist or do not belong to users in this table.")
                .With(Key.Error, GameTableCodes.SheetsNotExists);

        gameTable.SyncSheets(validSheetIds);

        await unitOfWork.CommitAsync();

        return Result.Ok();
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var gameTable = await gameTableRepository.GetAsync(id, userContext.Id);

        if (gameTable is null)
            return new NotFoundError("Table not found")
                .With(Key.Error, GameTableCodes.TableNotFound);

        gameTableRepository.Delete(gameTable);
        await unitOfWork.CommitAsync();

        return Result.Ok();
    }
}