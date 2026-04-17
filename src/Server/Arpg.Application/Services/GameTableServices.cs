using Arpg.Application.Auth;
using Arpg.Application.Mapper;
using Arpg.Application.Repositories;
using Arpg.Contracts.Dto.GameTable;
using Arpg.Primitives.Codes;
using Arpg.Primitives.Constants;
using Arpg.Primitives.Results;
using FluentValidation;

namespace Arpg.Application.Services;

public class GameTableServices(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    IGameTableRepository gameTableRepository,
    IUserRepository userRepository,
    ISheetRepository sheetRepository,
    IValidator<NewDto> newDtoValidator,
    IValidator<GameTableOperationDto> batchPlayersDtoValidator
) : BaseService
{
    private readonly GameTableMapper _gameTableMapper = new();

    public async Task<Result<GameTableDto>> CreateAsync(NewDto request)
    {
        var validation = Validate(newDtoValidator, request);
        if (validation.IsFailed)
            return validation;

        var gameTable = _gameTableMapper.ToGameTable(request);

        gameTable.SetGameMaster(userContext.Id);

        gameTableRepository.Add(gameTable);
        await unitOfWork.CommitAsync();

        return Result.Ok(_gameTableMapper.ToGameTableDto(gameTable));
    }

    public async Task<Result> UpdatePlayersAsync(GameTableBatchDto request)
    {
        var validation = Validate(batchPlayersDtoValidator, request.Batch);
        if (validation.IsFailed)
            return validation;

        var gameTable = await gameTableRepository.GetAsync(request.TableId, userContext.Id);

        if (gameTable is null)
            return Result.Fail(new NotFoundError("Table not found")
                .WithMetadata(MetadataKey.Error, GameTableCodes.TableNotFound));

        var playerIds = request.Batch.Select(dto => dto.Id);

        if (!await userRepository.AnyAll(playerIds))
            return Result.Fail(new UnprocessableEntityError("Players not exists")
                .WithMetadata(MetadataKey.Error, GameTableCodes.PlayersNotExists));

        var domainBatch = request.Batch.Select(dto => _gameTableMapper.ToGameTableOperation(dto));

        var result = gameTable.UpdatePlayers(domainBatch);
        if (result.IsFailed)
            return result;

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
            return Result.Fail(new NotFoundError("Table not found")
                .WithMetadata(MetadataKey.Error, GameTableCodes.TableNotFound));

        var sheetIds = request.Batch.Select(dto => dto.Id);

        if (!await sheetRepository.AnyAll(sheetIds))
            return Result.Fail(new UnprocessableEntityError("Sheets not exists")
                .WithMetadata(MetadataKey.Error, GameTableCodes.SheetsNotExists));

        var domainBatch = request.Batch.Select(dto => _gameTableMapper.ToGameTableOperation(dto));

        var result = gameTable.UpdateSheets(domainBatch);
        if (result.IsFailed)
            return result;

        await unitOfWork.CommitAsync();

        return Result.Ok();
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var gameTable = await gameTableRepository.GetAsync(id, userContext.Id);

        if (gameTable is null)
            return Result.Fail(new NotFoundError("Table not found")
                .WithMetadata(MetadataKey.Error, GameTableCodes.TableNotFound));

        gameTableRepository.Delete(gameTable);
        await unitOfWork.CommitAsync();

        return Result.Ok();
    }
}