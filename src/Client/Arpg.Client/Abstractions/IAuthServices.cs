using System.Threading.Tasks;

using Arpg.Contracts.Dto.User;

using FluentResults;

namespace Arpg.Client.Abstractions;

public interface IAuthServices
{
    Task<Result<CodeDto>> LoginAsync(LoginDto request);
    Task<Result<CodeDto>> NewAsync(NewUserDto request);
    Task<Result> ValidateAsync(ValidateCodeDto request);
}
