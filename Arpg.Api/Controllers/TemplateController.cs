using Arpg.Application.Services;

using Arpg.Contracts.Dto.General;
using Arpg.Contracts.Dto.Template;

using Microsoft.AspNetCore.Mvc;

namespace Arpg.Api.Controllers;

public class TemplateController(TemplateServices templateServices) : BaseController()
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TemplateCreateDto request)
    {
        var result = await templateServices.CreateAsync(request);

        if (result.IsFailed)
            return ToFailResults(result);

        return CreatedAtAction(
            nameof(Get),
            new { id = result.Value},
            new SuccessCreateDto("Template created successfully.", result.Value)
        );
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await templateServices.GetAsync(id);

        return result.IsFailed ? ToFailResults(result) : Ok(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        var result = await templateServices.GetListAsync();

        return result.IsFailed ? ToFailResults(result) : Ok(result.Value);
    }

    [HttpPut]
    public async Task<IActionResult> Edit([FromBody] TemplateEditDto request)
    {
        var result = await templateServices.EditAsync(request);

        return result.IsFailed ? ToFailResults(result) : Ok(new SuccessDto("Template edited successfully."));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] TemplateDeleteDto request)
    {
        var result = await templateServices.DeleteAsync(request);

        return result.IsFailed ? ToFailResults(result) : Ok(new SuccessDto("Template deleted successfully."));
    }
}