namespace Arpg.Contracts.Dto.Sheet;

public record SheetCreateDto(string Name, Guid TemplateId);
public record SheetEditDto(Guid Id, string Name);
public record ComputeSheetDto(Guid Id, Dictionary<Guid, object?> Data);
public record SheetListDto(Guid Id, string Name);
public record SheetDto
(
    Guid Id,
    Guid OwnerId,
    string Name,
    Dictionary<Guid, object?> Data,
    DateTime CreateAt
);