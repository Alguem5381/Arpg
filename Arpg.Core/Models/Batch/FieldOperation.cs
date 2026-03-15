using Arpg.Shared.Enums.Template;

namespace Arpg.Core.Models.Batch;

public class FieldOperation
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