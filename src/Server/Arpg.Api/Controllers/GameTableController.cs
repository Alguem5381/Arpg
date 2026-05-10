using Microsoft.AspNetCore.Mvc;
using Arpg.Application.Auth;
using Arpg.Application.Queries;
using Arpg.Application.Services;
using Arpg.Contracts.Dto.GameTable;
using Arpg.Contracts.Dto.General;
using Arpg.Contracts.Dto.Sheet;
using Arpg.Contracts.Dto.User;

namespace Arpg.Api.Controllers;

public class GameTableController(
    GameTableServices gameTableServices,
    IGameTableQueries gameTableQueries,
    IUserContext userContext
) : BaseController
{
    /// <summary>
    /// Cria uma nova mesa.
    /// </summary>
    /// <param name="request">Novo dto de mesa.</param>
    /// <response code="201">Mesa criada com sucesso.</response>
    /// <response code="400">**Validation**: Erro de validação.</response>
    /// <response code="401">**Unauthorized**: Usuário não autenticado.</response>
    /// <returns>Dto da mesa criada.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(GameTableDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] NewTableDto request)
    {
        var result = await gameTableServices.CreateAsync(request);

        return result.IsFailed ? ToFailResults(result) : StatusCode(StatusCodes.Status201Created, result.Value);
    }

    /// <summary>
    /// Adiciona ou remove jogadores da mesa.
    /// </summary>
    /// <param name="request">Lote de operações de jogadores.</param>
    /// <response code="200">Operação realizada com sucesso.</response>
    /// <response code="400">**Validation**: Erro de validação.</response>
    /// <response code="401">**Unauthorized**: Usuário não autenticado.</response>
    /// <response code="404">**NotFound**: Mesa não encontrada.</response>
    /// <response code="422">**UnprocessableEntity**: Jogador não existe ou operações inválidas.</response>
    [HttpPost("user")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateUsers([FromBody] GameTableBatchDto request)
    {
        var result = await gameTableServices.UpdateUsersAsync(request);

        return result.IsFailed ? ToFailResults(result) : Ok();
    }

    /// <summary>
    /// Adiciona ou remove fichas da mesa.
    /// </summary>
    /// <param name="request">Lote de operações de fichas.</param>
    /// <response code="200">Operação realizada com sucesso.</response>
    /// <response code="400">**Validation**: Erro de validação.</response>
    /// <response code="401">**Unauthorized**: Usuário não autenticado.</response>
    /// <response code="404">**NotFound**: Mesa não encontrada.</response>
    /// <response code="422">**UnprocessableEntity**: Ficha não existe ou operações inválidas.</response>
    [HttpPost("sheet")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateSheets([FromBody] GameTableBatchDto request)
    {
        var result = await gameTableServices.UpdateSheetsAsync(request);

        return result.IsFailed ? ToFailResults(result) : Ok();
    }

    /// <summary>
    /// Remove uma mesa.
    /// </summary>
    /// <param name="tableId">Identificador da mesa.</param>
    /// <response code="204">Mesa removida com sucesso.</response>
    /// <response code="401">**Unauthorized**: Usuário não autenticado.</response>
    /// <response code="404">**NotFound**: Mesa não encontrada.</response>
    [HttpDelete("{tableId:guid}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid tableId)
    {
        var result = await gameTableServices.DeleteAsync(tableId);

        return result.IsFailed ? ToFailResults(result) : NoContent();
    }

    /// <summary>
    /// Retorna todas as mesas do mestre autenticado.
    /// </summary>
    /// <response code="200">Lista de mesas retornada com sucesso.</response>
    /// <response code="401">**Unauthorized**: Usuário não autenticado.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<SimpleGameTableDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
        => Ok(await gameTableQueries.GetAllAsync(userContext.Id));

    /// <summary>
    /// Retorna todos os jogadores de uma mesa (versão simplificada).
    /// </summary>
    /// <param name="tableId">Identificador da mesa.</param>
    /// <response code="200">Lista de jogadores retornada com sucesso.</response>
    /// <response code="401">**Unauthorized**: Usuário não autenticado.</response>
    [HttpGet("{tableId:guid}/users")]
    [ProducesResponseType(typeof(List<UserInformationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlayers([FromRoute] Guid tableId)
        => Ok(await gameTableQueries.GetUsersAsync(tableId, userContext.Id));

    /// <summary>
    /// Retorna todas as fichas de uma mesa (visão do mestre).
    /// </summary>
    /// <param name="tableId">Identificador da mesa.</param>
    /// <response code="200">Lista de fichas retornada com sucesso.</response>
    /// <response code="401">**Unauthorized**: Usuário não autenticado.</response>
    [HttpGet("{tableId:guid}/sheets")]
    [ProducesResponseType(typeof(List<SimpleSheetDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllSheets([FromRoute] Guid tableId)
        => Ok(await gameTableQueries.GetAllSheetsAsync(tableId, userContext.Id));

    /// <summary>
    /// Retorna as fichas do jogador autenticado em uma mesa.
    /// </summary>
    /// <param name="tableId">Identificador da mesa.</param>
    /// <response code="200">Lista de fichas do jogador retornada com sucesso.</response>
    /// <response code="401">**Unauthorized**: Usuário não autenticado.</response>
    [HttpGet("{tableId:guid}/my-sheets")]
    [ProducesResponseType(typeof(List<SimpleSheetDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMySheets([FromRoute] Guid tableId)
        => Ok(await gameTableQueries.GetUserSheetsAsync(tableId, userContext.Id));
}