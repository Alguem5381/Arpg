namespace Arpg.Client.Abstractions;

public interface IUserSession
{
    string? Token { get; set; }
    bool IsAuthenticated => !string.IsNullOrEmpty(Token);
}
