using Arpg.Core.Models.Tabletop.Batch;
using Arpg.Primitives.Codes;
using Arpg.Primitives.Constants;
using Arpg.Primitives.Enums.Template;
using Arpg.Primitives.Results;

namespace Arpg.Core.Models.Tabletop;

public class GameTable
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public Guid DmId { get; private set; } = Guid.Empty;
    public Guid TemplateId { get; private set; }
    public List<Guid> PlayerIds { get; private set; } = [];
    public List<Guid> SheetIds { get; private set; } = [];
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public GameTable()
    {
    }

    public GameTable(string name, Guid dmId, Guid templateId)
    {
        Name = name;
        DmId = dmId;
        TemplateId = templateId;
    }

    public void SetGameMaster(Guid dmId)
    {
        if (DmId == Guid.Empty)
            DmId = dmId;
    }

    public Result UpdatePlayers(IEnumerable<GameTableOperation> batch)
    {
        List<Error> errors = [];

        foreach (var operation in batch)
        {
            switch (operation.Operation)
            {
                case Operation.Add when PlayerIds.Contains(operation.Id):
                    errors.Add(new UnprocessableEntityError("Player already exists on table.")
                        .WithMetadata(MetadataKey.Error, GameTableCodes.PlayerAlreadyOnTable)
                        .WithMetadata(MetadataKey.Id, operation.Id)
                        .WithMetadata(MetadataKey.Operation, operation.Operation));
                    break;
                case Operation.Add:
                    PlayerIds.Add(operation.Id);
                    break;
                case Operation.Delete when !PlayerIds.Contains(operation.Id):
                    errors.Add(new UnprocessableEntityError("Player not exists on table.")
                        .WithMetadata(MetadataKey.Error, GameTableCodes.PlayerNotOnTable)
                        .WithMetadata(MetadataKey.Id, operation.Id)
                        .WithMetadata(MetadataKey.Operation, operation.Operation));
                    break;
                case Operation.Delete:
                    PlayerIds.Remove(operation.Id);
                    break;
                default:
                    errors.Add(new UnprocessableEntityError("This operation is invalid in this context.")
                        .WithMetadata(MetadataKey.Error, GeneralCodes.InvalidOperation)
                        .WithMetadata(MetadataKey.Id, operation.Id)
                        .WithMetadata(MetadataKey.Operation, operation.Operation));
                    break;
            }
        }

        return errors.Count != 0 ? Result.Fail(errors) : Result.Ok();
    }

    public Result UpdateSheets(IEnumerable<GameTableOperation> batch)
    {
        List<Error> errors = [];

        foreach (var operation in batch)
        {
            switch (operation.Operation)
            {
                case Operation.Add when SheetIds.Contains(operation.Id):
                    errors.Add(new UnprocessableEntityError("Sheet already exists on table.")
                        .WithMetadata(MetadataKey.Error, GameTableCodes.SheetAlreadyOnTable)
                        .WithMetadata(MetadataKey.Id, operation.Id)
                        .WithMetadata(MetadataKey.Operation, operation.Operation));
                    break;
                case Operation.Add:
                    SheetIds.Add(operation.Id);
                    break;
                case Operation.Delete when !SheetIds.Contains(operation.Id):
                    errors.Add(new UnprocessableEntityError("Sheet not exists on table.")
                        .WithMetadata(MetadataKey.Error, GameTableCodes.SheetNotOnTable)
                        .WithMetadata(MetadataKey.Id, operation.Id)
                        .WithMetadata(MetadataKey.Operation, operation.Operation));
                    break;
                case Operation.Delete:
                    SheetIds.Remove(operation.Id);
                    break;
                default:
                    errors.Add(new UnprocessableEntityError("This operation is invalid in this context.")
                        .WithMetadata(MetadataKey.Error, GeneralCodes.InvalidOperation)
                        .WithMetadata(MetadataKey.Id, operation.Id)
                        .WithMetadata(MetadataKey.Operation, operation.Operation));
                    break;
            }
        }

        return errors.Count != 0 ? Result.Fail(errors) : Result.Ok();
    }
}

