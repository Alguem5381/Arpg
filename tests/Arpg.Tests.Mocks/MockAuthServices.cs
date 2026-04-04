using System;
using System.Threading.Tasks;
using Arpg.Client.Abstractions;
using Arpg.Contracts.Dto.User;
using FluentResults;

namespace Arpg.Tests.Mocks;

public class MockAuthServices(IUserSession userSession) : IAuthServices
{
    public async Task<Result<CodeDto>> LoginAsync(LoginDto request)
    {
        // Simulate network delay
        await Task.Delay(1500);

        if (request.Username == "error")
        {
            return Result.Fail("Usuário ou senha inválidos (Mock Error)");
        }

        return Result.Ok(new CodeDto(Guid.NewGuid()));
    }

    public async Task<Result<CodeDto>> NewAsync(NewDto request)
    {
        await Task.Delay(1500);

        if (request.Username == "error")
        {
            return Result.Fail("Erro ao criar conta (Mock Error)");
        }

        return Result.Ok(new CodeDto(Guid.NewGuid()));
    }

    public async Task<Result> ValidateAsync(ValidateCodeDto request)
    {
        await Task.Delay(1000);

        if (request.Value == "000000")
        {
            return Result.Fail("Código inválido (Mock Error)");
        }

        userSession.Token = "mock_jwt_token_" + Guid.NewGuid().ToString();
        return Result.Ok();
    }
}
