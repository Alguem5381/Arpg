namespace Arpg.Contracts.Dto.Sheet;

public record NewSheetDto(string Name, Guid TemplateId);

public record EditSheetDto(Guid Id, string Name);

public record ComputeSheetDto(Guid Id, Dictionary<Guid, object?> Data);

public record SimpleSheetDto(Guid Id, string Name);

public record SheetDto(
    Guid Id,
    Guid OwnerId,
    string Name,
    Dictionary<Guid, object?> Data,
    DateTime CreateAt
);