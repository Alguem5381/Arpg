using Arpg.Contracts.Dto.General;
using Arpg.Primitives.Enums.Template;

namespace Arpg.Contracts.Dto.Structure;

public class BatchUpdateDto
{
    public Guid TemplateId { get; set; }
    public List<CategoryOpDto>? Categories { get; set; } = [];
    public List<FieldOpDto>? Fields { get; set; } = [];
}

public class CategoryOpDto
{
    public Operation Op { get; set; }
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public int? Order { get; set; }
}

public class FieldOpDto
{
    public Operation Op { get; set; }
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public string? Name { get; set; }
    public FieldType? Type { get; set; }
    public bool SetDefaultValueToNull { get; set; }
    public object? DefaultValue { get; set; }
    public bool? IsRequired { get; set; }
}

public record StructureErrorDto(string Message, List<ErrorDto> Erros);