namespace Arpg.Core.Models.Customer;

public class Code
{
    public Guid OwnerId { get; init; }
    public Guid Key { get; } = Guid.NewGuid();
    public int Value { get; } = Random.Shared.Next(100000, 999999);
}