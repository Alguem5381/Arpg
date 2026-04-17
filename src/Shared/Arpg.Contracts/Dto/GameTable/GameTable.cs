using Arpg.Primitives.Enums.Template;

namespace Arpg.Contracts.Dto.GameTable;

public record NewDto(string Name, Guid TemplateId);

public record SimpleGameTableDto(Guid Id, string Name, Guid DmId, Guid TemplateId, DateTime CreatedAt);

public record GameTableBatchDto(Guid TableId, List<GameTableOperationDto> Batch);

public record GameTableDto(
    Guid Id,
    string Name,
    Guid DmId,
    Guid TemplateId,
    List<Guid> PlayerIds,
    List<Guid> SheetIds,
    DateTime CreatedAt);

public record GameTableOperationDto(Operation Operation, Guid Id);