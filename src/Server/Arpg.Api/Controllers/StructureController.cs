using Arpg.Application.Services;
using Arpg.Contracts.Dto.General;
using Arpg.Contracts.Dto.Structure;
using Microsoft.AspNetCore.Mvc;

namespace Arpg.Api.Controllers;

public class StructureController(StructureServices structureServices) : BaseController
{
    /// <summary>
    /// Atualiza a estrutura de um template.
    /// </summary>
    /// <param name="request">A requisição de atualização da estrutura.</param>
    /// <returns>Uma mensagem de sucesso se a atualização for bem-sucedida, ou uma mensagem de erro se a atualização falhar.</returns>
    /// <response code="200">A atualização foi bem-sucedida.</response>
    /// <response code="401">**Unauthorized**: Usuário não autenticado.</response>
    /// <response code="404">**NotFound**: O template não foi encontrado.</response>
    /// <response code="422">**UnprocessableEntity**: A atualização falhou devido a erros de validação.</response>
    [HttpPut("update")]
    [ProducesResponseType(typeof(SuccessDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateStructure([FromBody] BatchStructureDto request)
    {
        var result = await structureServices.UpdateStructureAsync(request);

        return result.IsSuccess
            ? Ok(new SuccessDto("Template structure updated successfully."))
            : ToFailResults(result);
    }
}