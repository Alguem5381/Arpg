using Arpg.Core.Models.Definitions;

namespace Arpg.Contracts.Dto.Template;

public record SuccessCreateDto(string Message, Guid Id);

public record NewTemplateDto(string Name);

public record DeleteTemplateDto(Guid Id, string Password);

public record EditTemplateDto(Guid Id, string Name, string? Description);

public record SimpleTemplateDto(Guid Id, string Name, string? Description, DateTime CreationTime);

public record TemplateDto(
    Guid Id,
    Guid? OwnerId,
    string Name,
    string? Description,
    bool IsArchived,
    TemplateStructure? Structure,
    DateTime CreationTime
);