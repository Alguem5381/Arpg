using Arpg.Core.Models;
using Arpg.Core.Models.Customer;

namespace Arpg.Application.Auth;

public interface ITokenServices
{
    string GenerateToken(User user, string username);
}