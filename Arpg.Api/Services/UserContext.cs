using System.Security.Claims;
using Arpg.Application.Abstractions;
using Arpg.Application.Auth;

namespace Arpg.Api.Services;

public class UserContext(IHttpContextAccessor accessor) : IUserContext
{
    public Guid Id 
        => Guid.TryParse(accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out var guid) ? guid : Guid.Empty;

    public bool IsAuthenticated 
        => accessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}