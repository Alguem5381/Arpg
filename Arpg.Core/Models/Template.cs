namespace Arpg.Core.Models;

public class Template
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? OwnerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public bool IsArchived { get; set; }
    public DateTime CreationTime { get; set; } = DateTime.UtcNow;

    public TemplateStructure Structure { get; set; } = new();
}
