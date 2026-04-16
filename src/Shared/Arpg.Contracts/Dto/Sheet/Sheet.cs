namespace Arpg.Contracts.Dto.Sheet;

public record CreateDto(string Name, Guid TemplateId);
public record EditDto(Guid Id, string Name);
public record ComputeDto(Guid Id, Dictionary<Guid, object?> Data);
public record SimpleSheetDto(Guid Id, string Name);
public record SheetDto
(
    Guid Id,
    Guid OwnerId,
    string Name,
    Dictionary<Guid, object?> Data,
    DateTime CreateAt
);