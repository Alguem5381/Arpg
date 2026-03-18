using Arpg.Client.Abstractions;

namespace Arpg.Tests.Mocks;

public class MockUserSession : IUserSession
{
    public string? Token { get; set; }
    public bool IsAuthenticated => !string.IsNullOrEmpty(Token);
}
