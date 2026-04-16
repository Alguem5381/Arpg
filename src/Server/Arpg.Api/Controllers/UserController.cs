using Arpg.Application.Auth;
using Arpg.Application.Services;

using Arpg.Contracts.Dto.General;
using Arpg.Contracts.Dto.User;
using Arpg.Infrastructure.Queries;
using Arpg.Primitives.Codes;
using Arpg.Primitives.Constants;
using Arpg.Primitives.Results;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Arpg.Api.Controllers;

public class UserController
(
    UserServices userServices,
    AccountServices accountServices,
    UserQueries userQueries,
    IUserContext userContext
) : BaseController
{
    /// <summary>
    /// Cria uma nova conta de usuário.
    /// </summary>
    /// <param name="request">Dados da nova conta (Username, Email, Senha).</param>
    /// <returns>Um código de verificação para validar a conta.</returns>
    /// <response code="200">Conta criada com sucesso.</response>
    /// <response code="400">**ValidationError**: Dados inválidos enviados na requisição.</response>
    /// <response code="409">**ConflictError**: Nome de usuário já cadastrado.</response>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CodeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> New([FromBody] NewDto request)
    {
        var result = await accountServices.NewAsync(request);

        return result.IsFailed ? ToFailResults(result) : Ok(result.Value);
    }

    /// <summary>
    /// Faz login na conta de usuário.
    /// </summary>
    /// <param name="request">Dados do login (Username, Senha).</param>
    /// <returns>Um código de verificação para validar a conta.</returns>
    /// <response code="200">Código de verificação enviado com sucesso.</response>
    /// <response code="400">**ValidationError**: Dados inválidos enviados na requisição.</response>
    /// <response code="401">**UnauthorizedError**: Credenciais inválidas ou conta bloqueada.</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CodeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        var result = await accountServices.LoginAsync(request);

        return result.IsFailed ? ToFailResults(result) : Ok(result.Value);
    }

    /// <summary>
    /// Valida um código de verificação e gera um token JWT.
    /// </summary>
    /// <param name="request">Código de verificação e chave.</param>
    /// <returns>Token JWT.</returns>
    /// <response code="200">Código de verificação validado com sucesso.</response>
    /// <response code="400">**Validation**: Dados inválidos enviados na requisição.</response>
    /// <response code="404">**NotFound**: Código não encontrado.</response>
    /// <response code="422">**UnprocessableEntity**: Não é possível resolver este código.</response>
    /// <response code="401">**Unauthorized**: Código inválido.</response>
    [HttpPost("validate")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(SuccessLoginDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Validate([FromBody] ValidateCodeDto request)
    {
        var result = await accountServices.ValidateCode(request);

        return result.IsFailed ? ToFailResults(result) : Ok(new SuccessLoginDto("Logged in successfully.", result.Value));
    }

    /// <summary>
    /// Obtém informações de um usuário pelo nome de usuário.
    /// </summary>
    /// <param name="username">Nome de usuário do usuário.</param>
    /// <returns>Informações do usuário.</returns>
    /// <response code="200">Informações do usuário obtidas com sucesso.</response>
    /// <response code="404">**NotFound**: Usuário não encontrado.</response>
    [HttpGet("{username}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserInformationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFlat(string username)
    {
        var user = await userQueries.GetSimpleAsync(username);

        if (user == null)
            return ToFailResults(Result.Fail(new NotFoundError("Username not found.")
                    .WithMetadata(MetadataKey.Error, UserCodes.UserNotFound)));

        return Ok(user);
    }

    /// <summary>
    /// Obtém informações do usuário logado.
    /// </summary>
    /// <returns>Informações do usuário.</returns>
    /// <response code="200">Informações do usuário obtidas com sucesso.</response>
    /// <response code="404">**NotFound**: Usuário não encontrado.</response>
    /// <response code="401">**Unauthorized**: Usuário não autenticado.</response>
    [HttpGet]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSelf()
    {
        var result = await userQueries.GetSelfAsync(userContext.Id);

        if (result == null)
            return ToFailResults(Result.Fail(new NotFoundError("User not found.")
                .WithMetadata(MetadataKey.Error, UserCodes.UserNotFound)));

        return Ok(result);
    }

    /// <summary>
    /// Edita as informações do usuário logado.
    /// </summary>
    /// <param name="request">Dados do usuário.</param>
    /// <returns>Informações do usuário.</returns>
    /// <response code="200">Informações do usuário editadas com sucesso.</response>
    /// <response code="400">**Validation**: Dados inválidos enviados na requisição.</response>
    /// <response code="404">**NotFound**: Usuário não encontrado.</response>
    /// <response code="401">**Unauthorized**: Usuário não autenticado.</response>
    [HttpPut]
    [ProducesResponseType(typeof(SuccessDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Edit([FromBody] EditDto request)
    {
        var result = await userServices.EditAsync(request);

        return result.IsFailed ? ToFailResults(result) : Ok(new SuccessDto($"Name change successfully to {result.Value.DisplayName}."));
    }

    /// <summary>
    /// Deleta a conta do usuário logado.
    /// </summary>
    /// <param name="request">Dados do usuário.</param>
    /// <returns>Informações do usuário.</returns>
    /// <response code="200">Informações do usuário deletadas com sucesso.</response>
    /// <response code="400">**Validation**: Dados inválidos enviados na requisição.</response>
    /// <response code="401">**Unauthorized**: Usuário não autenticado.</response>
    /// <response code="422">**UnprocessableEntity**: Senha incorreta.</response>
    /// <response code="404">**NotFound**: A conta do usuário não foi encontrada.</response>
    [HttpDelete]
    [ProducesResponseType(typeof(SuccessDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Delete([FromBody] DeleteDto request)
    {
        var result = await accountServices.DeleteAsync(request);

        return result.IsFailed ? ToFailResults(result) : Ok(new SuccessDto("User deleted successfully."));
    }
}