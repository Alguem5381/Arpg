namespace Arpg.Application.Auth;

public interface IUserContext
{
    Guid Id { get; }
    bool IsAuthenticated { get; }
}