using Arpg.Application.Services;

using Arpg.Contracts.Dto.Sheet;

using Arpg.Primitives.Codes;
using Arpg.Primitives.Constants;
using Arpg.Primitives.Results;

using FluentResults;

using Microsoft.AspNetCore.Mvc;

namespace Arpg.Api.Controllers;

public class SheetController(SheetServices sheetServices) : BaseController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SheetCreateDto request)
    {
        var result = await sheetServices.Create(request);

        return result.IsFailed ? ToFailResults(result) : CreatedAtAction
        (
            nameof(Get),
            new { id = result.Value },
            result.Value
        );
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await sheetServices.GetAsync(id);

        return result.IsFailed ? ToFailResults(result) : Ok(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        var result = await sheetServices.GetListAsync();

        return result.IsFailed ? ToFailResults(result) : Ok(result.Value);
    }

    [HttpPut]
    public async Task<IActionResult> Edit([FromBody] SheetEditDto request)
    {
        var result = await sheetServices.EditAsync(request);

        return result.IsFailed ? ToFailResults(result) : Ok(result.Value);
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] ComputeSheetDto request)
    {
        var result = await sheetServices.ComputeDataAsync(request);

        return result.IsFailed ? ToFailResults(result) : Ok();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await sheetServices.DeleteAsync(id);

        return result.IsFailed ? ToFailResults(result) : NoContent();
    }
}