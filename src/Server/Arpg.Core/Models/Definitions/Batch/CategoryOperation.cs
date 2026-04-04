using Arpg.Primitives.Enums.Template;

namespace Arpg.Core.Models.Definitions.Batch;

public class CategoryOperation
{
    public Operation Op { get; set; }
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public int? Order { get; set; }
}