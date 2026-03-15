using System.Threading.Tasks;

using Arpg.Contracts.Dto.User;

using FluentResults;

namespace Arpg.Client.Abstractions;

public interface IAuthServices
{
    Task<Result> LoginAsync(LoginDto request);
    Task<Result> New(NewDto request);
}
