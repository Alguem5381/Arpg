using Arpg.Core.Models.Definitions;

namespace Arpg.Contracts.Dto.Template;

public record SuccessCreateDto(string Message, Guid Id);
public record TemplateCreateDto(string Name);
public record TemplateDeleteDto(Guid Id, string Password);
public record TemplateEditDto(Guid Id, string Name, string? Description);

public record FlatTemplateDto(Guid Id, string Name, string? Description, DateTime CreationTime);
public record TemplateDto(
    Guid Id,
    Guid? OwnerId,
    string Name,
    string? Description,
    bool IsArchived,
    TemplateStructure? Structure,
    DateTime CreationTime
    );