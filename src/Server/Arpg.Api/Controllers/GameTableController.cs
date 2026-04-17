using Microsoft.AspNetCore.Mvc;
using Arpg.Application.Services;
using Arpg.Contracts.Dto.GameTable;
using Arpg.Contracts.Dto.General;

namespace Arpg.Api.Controllers;

public class GameTableController(GameTableServices gameTableServices) : BaseController
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
    public async Task<IActionResult> Create([FromBody] NewDto request)
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
    [HttpPost("player")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdatePlayers([FromBody] GameTableBatchDto request)
    {
        var result = await gameTableServices.UpdatePlayersAsync(request);

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
}