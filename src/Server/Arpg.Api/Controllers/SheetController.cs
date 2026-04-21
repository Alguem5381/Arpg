using Arpg.Application.Auth;
using Arpg.Application.Queries;
using Arpg.Application.Services;
using Arpg.Contracts.Dto.General;
using Arpg.Contracts.Dto.Sheet;
using Arpg.Primitives.Codes;
using Arpg.Primitives.Constants;
using Arpg.Primitives.Results;
using Microsoft.AspNetCore.Mvc;

namespace Arpg.Api.Controllers;

public class SheetController(
    SheetServices sheetServices,
    ISheetQueries sheetQueries,
    IUserContext userContext
) : BaseController
{
    /// <summary>
    /// Cria uma nova ficha.
    /// </summary>
    /// <param name="request">A ficha para criar.</param>
    /// <returns>A ficha criada.</returns>
    /// <response code="201">A ficha criada.</response>
    /// <response code="400">**BadRequest**: Requisição inválida.</response>
    /// <response code="401">**Unauthorized**: Usuário não autenticado.</response>
    /// <response code="404">**NotFound**: O template não foi encontrado.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] NewSheetDto request)
    {
        var result = await sheetServices.CreateAsync(request);

        return result.IsFailed
            ? ToFailResults(result)
            : CreatedAtAction
            (
                nameof(Get),
                new { id = result.Value },
                result.Value
            );
    }

    /// <summary>
    /// Busca uma ficha pelo ID.
    /// </summary>
    /// <param name="id">O ID da ficha.</param>
    /// <returns>A ficha.</returns>
    /// <response code="200">A ficha.</response>
    /// <response code="401">**Unauthorized**: Usuário não autenticado.</response>
    /// <response code="404">**NotFound**: A ficha não foi encontrada.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SheetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id)
    {
        var sheet = await sheetQueries.GetSheetDtoAsync(id, userContext.Id);

        if (sheet == null)
            return ToFailResults(Result.Fail(new NotFoundError("Sheet not found.")
                .WithMetadata(MetadataKey.Error, SheetCodes.SheetNotFound)));

        return Ok(sheet);
    }

    /// <summary>
    /// Busca uma lista de fichas do usuário.
    /// </summary>
    /// <returns>A lista de fichas.</returns>
    /// <response code="200">A lista de fichas.</response>
    /// <response code="401">**Unauthorized**: Usuário não autenticado.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<SimpleSheetDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetList()
    {
        var sheets = await sheetQueries.GetSimpleListAsync(userContext.Id);

        return Ok(sheets);
    }

    /// <summary>
    /// Edita uma ficha.
    /// </summary>
    /// <param name="request">A ficha para editar.</param>
    /// <returns>A ficha editada.</returns>
    /// <response code="200">A ficha editada.</response>
    /// <response code="401">**Unauthorized**: Usuário não autenticado.</response>
    /// <response code="404">**NotFound**: A ficha não foi encontrada.</response>
    /// <response code="400">**BadRequest**: Requisição inválida.</response>
    [HttpPut]
    [ProducesResponseType(typeof(SheetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Edit([FromBody] EditSheetDto request)
    {
        var result = await sheetServices.EditAsync(request);

        return result.IsFailed ? ToFailResults(result) : Ok(result.Value);
    }

    /// <summary>
    /// Atualiza os dados de uma ficha.
    /// </summary>
    /// <param name="request">A ficha para atualizar.</param>
    /// <returns>A ficha atualizada.</returns>
    /// <response code="200">A ficha atualizada.</response>
    /// <response code="401">**Unauthorized**: Usuário não autenticado.</response>
    /// <response code="404">**NotFound**: A ficha não foi encontrada.</response>
    /// <response code="400">**BadRequest**: Requisição inválida.</response>
    /// <response code="422">**UnprocessableEntity**: Dados inválidos.</response>
    [HttpPut("update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update([FromBody] ComputeSheetDto request)
    {
        var result = await sheetServices.ComputeDataAsync(request);

        return result.IsFailed ? ToFailResults(result) : Ok();
    }

    /// <summary>
    /// Deleta uma ficha.
    /// </summary>
    /// <param name="id">O ID da ficha.</param>
    /// <returns>A ficha deletada.</returns>
    /// <response code="204">A ficha deletada com sucesso.</response>
    /// <response code="401">**Unauthorized**: Usuário não autenticado.</response>
    /// <response code="404">**NotFound**: A ficha não foi encontrada.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await sheetServices.DeleteAsync(id);

        return result.IsFailed ? ToFailResults(result) : NoContent();
    }
}