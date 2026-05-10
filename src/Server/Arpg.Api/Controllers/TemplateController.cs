using Arpg.Application.Services;
using Arpg.Contracts.Dto.General;
using Arpg.Contracts.Dto.Template;
using Arpg.Application.Queries;
using Arpg.Application.Auth;
using Arpg.Primitives.Constants;
using Arpg.Primitives.Codes;
using Microsoft.AspNetCore.Mvc;
using Arpg.Primitives.Results;

namespace Arpg.Api.Controllers;

public class TemplateController(
    TemplateServices templateServices,
    ITemplateQueries templateQueries,
    IUserContext userContext
) : BaseController
{
    /// <summary>
    /// Cria um novo Template de RPG associado ao usuário
    /// </summary>
    /// <param name="request">Dados do template a ser criado</param>
    /// <returns>ID do Template criado</returns>
    /// <response code="201">Template criado com sucesso.</response>
    /// <response code="400">**Validation**: Dados inválidos enviados na requisição (ex: nome muito curto).</response>
    /// <response code="401">**Unauthorized**: Usuário não autenticado.</response>
    [HttpPost]
    [ProducesResponseType(typeof(SuccessCreateDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] NewTemplateDto request)
    {
        var result = await templateServices.CreateAsync(request);

        if (result.IsFailed)
            return ToFailResults(result);

        return CreatedAtAction(
            nameof(Get),
            new { id = result.Value },
            new SuccessCreateDto("Template created successfully.", result.Value)
        );
    }

    /// <summary>
    /// Busca os detalhes e a estrutura de um Template pelo ID
    /// </summary>
    /// <param name="id">ID único do Template</param>
    /// <returns>Os dados completos do Template</returns>
    /// <response code="200">Retorna o DTO do Template de RPG com sua estrutura completa.</response>
    /// <response code="401">**Unauthorized**: Usuário não autenticado.</response>
    /// <response code="404">**NotFound**: O Template não existe ou não pertence ao usuário.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TemplateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var template = await templateQueries.GetTemplate(id, userContext.Id);

        if (template == null)
            return ToFailResults(Result.Fail(new NotFoundError("Template not found.")
                .With(Key.Error, TemplateCodes.TemplateNotFound)));

        return Ok(template);
    }

    /// <summary>
    /// Busca a lista de templates resumidos criados pelo usuário logado
    /// </summary>
    /// <returns>Lista de Templates sem a estrutura</returns>
    /// <response code="200">Retorna a lista de Templates.</response>
    /// <response code="401">**Unauthorized**: Usuário não autenticado.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<SimpleTemplateDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetList()
        => Ok(await templateQueries.GetListAsync(userContext.Id));

    /// <summary>
    /// Edita um Template existente
    /// </summary>
    /// <param name="request">Dados do Template a ser editado</param>
    /// <returns>Mensagem de sucesso</returns>
    /// <response code="200">Template editado com sucesso.</response>
    /// <response code="400">**Validation**: Dados inválidos enviados na requisição (ex: nome muito curto).</response>
    /// <response code="404">**NotFound**: O Template não existe ou não pertence ao usuário.</response>
    /// <response code="401">**Unauthorized**: Usuário não autenticado.</response>
    [HttpPut]
    [ProducesResponseType(typeof(SuccessDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Edit([FromBody] EditTemplateDto request)
    {
        var result = await templateServices.EditAsync(request);

        return result.IsFailed ? ToFailResults(result) : Ok(new SuccessDto("Template edited successfully."));
    }

    /// <summary>
    /// Deleta um Template existente
    /// </summary>
    /// <param name="request">Dados do Template a ser deletado</param>
    /// <returns>Mensagem de sucesso</returns>
    /// <response code="200">Template deletado com sucesso.</response>
    /// <response code="400">**Validation**: Dados inválidos enviados na requisição (ex: senha inválida).</response>
    /// <response code="404">**NotFound**: O Template não existe ou não pertence ao usuário.</response>
    /// <response code="401">**Unauthorized**: Usuário não autenticado.</response>
    /// <response code="422">**UnprocessableEntity**: Senha incorreta.</response>
    [HttpDelete]
    [ProducesResponseType(typeof(SuccessDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Delete([FromBody] DeleteTemplateDto request)
    {
        var result = await templateServices.DeleteAsync(request);

        return result.IsFailed ? ToFailResults(result) : Ok(new SuccessDto("Template deleted successfully."));
    }
}