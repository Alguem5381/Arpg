namespace Arpg.Core.Models.Customer;

public class Code
{
    public Guid OwnerId { get; init; }
    public Guid Key { get; } = Guid.NewGuid();
    public string Value { get; set; } = Random.Shared.Next(0, 999999).ToString("D6");
}