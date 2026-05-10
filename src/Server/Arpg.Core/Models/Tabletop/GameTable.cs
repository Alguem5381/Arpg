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
    public List<Guid> UserIds { get; private set; } = [];
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
        {
            DmId = dmId;
            UserIds.Add(dmId);
        }
    }

    public Result UpdateUsers(IEnumerable<GameTableOperation> batch)
    {
        List<Error> errors = [];

        foreach (var operation in batch)
        {
            switch (operation.Operation)
            {
                case Operation.Add when UserIds.Contains(operation.Id):
                    errors.Add(new UnprocessableError("User already exists in table.")
                        .With(Key.Error, GameTableCodes.UserAlreadyInTable)
                        .With(Key.Id, operation.Id)
                        .With(Key.Operation, operation.Operation));
                    break;

                case Operation.Add:
                    UserIds.Add(operation.Id);
                    break;

                case Operation.Delete when operation.Id == DmId:
                    errors.Add(new UnprocessableError("Cannot remove the Game Master from the table.")
                        .With(Key.Error, GameTableCodes.UserNotInTable)
                        .With(Key.Id, operation.Id)
                        .With(Key.Operation, operation.Operation));
                    break;

                case Operation.Delete when !UserIds.Contains(operation.Id):
                    errors.Add(new UnprocessableError("User not exists in table.")
                        .With(Key.Error, GameTableCodes.UserNotInTable)
                        .With(Key.Id, operation.Id)
                        .With(Key.Operation, operation.Operation));
                    break;

                case Operation.Delete:
                    UserIds.Remove(operation.Id);
                    break;

                default:
                    errors.Add(new UnprocessableError("This operation is invalid in this context.")
                        .With(Key.Error, GeneralCodes.InvalidOperation)
                        .With(Key.Id, operation.Id)
                        .With(Key.Operation, operation.Operation));
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
                    errors.Add(new UnprocessableError("Sheet already exists on table.")
                        .With(Key.Error, GameTableCodes.SheetAlreadyOnTable)
                        .With(Key.Id, operation.Id)
                        .With(Key.Operation, operation.Operation));
                    break;
                case Operation.Add:
                    SheetIds.Add(operation.Id);
                    break;
                case Operation.Delete when !SheetIds.Contains(operation.Id):
                    errors.Add(new UnprocessableError("Sheet not exists on table.")
                        .With(Key.Error, GameTableCodes.SheetNotOnTable)
                        .With(Key.Id, operation.Id)
                        .With(Key.Operation, operation.Operation));
                    break;
                case Operation.Delete:
                    SheetIds.Remove(operation.Id);
                    break;
                default:
                    errors.Add(new UnprocessableError("This operation is invalid in this context.")
                        .With(Key.Error, GeneralCodes.InvalidOperation)
                        .With(Key.Id, operation.Id)
                        .With(Key.Operation, operation.Operation));
                    break;
            }
        }

        return errors.Count != 0 ? Result.Fail(errors) : Result.Ok();
    }

    public void SyncUsers(IEnumerable<Guid> validUserIds)
    {
        UserIds = validUserIds.ToList();
    }

    public void SyncSheets(IEnumerable<Guid> validSheetIds)
    {
        SheetIds = validSheetIds.ToList();
    }
}

