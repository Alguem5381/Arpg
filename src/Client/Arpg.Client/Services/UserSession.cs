using Arpg.Client.Abstractions;

namespace Arpg.Client.Services;

public class UserSession : IUserSession
{
    public string? Token { get; set; }
    public bool IsAuthenticated { get; set; }
}
