namespace Arpg.Core.Models.Batch;

public class BatchUpdate
{
    public Guid TemplateId { get; set; }
    public List<CategoryOperation>? Categories { get; set; } = [];
    public List<FieldOperation>? Fields { get; set; } = [];
}