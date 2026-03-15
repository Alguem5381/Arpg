namespace Arpg.Core.Models.Customer;

public class User
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string DisplayName { get; set; } = string.Empty;
    public List<Sheet> Sheets { get; private set; } = [];
}