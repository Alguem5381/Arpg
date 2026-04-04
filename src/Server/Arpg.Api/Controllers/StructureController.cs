using Arpg.Api.Extensions;
using Arpg.Application.Services;
using Arpg.Contracts.Dto.General;
using Arpg.Contracts.Dto.Structure;
using Arpg.Primitives.Codes;
using Arpg.Primitives.Constants;
using Arpg.Primitives.Results;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace Arpg.Api.Controllers;

public class StructureController(StructureServices structureServices) : BaseController
{
    [HttpPut("update")]
    public async Task<IActionResult> UpdateStructure([FromBody] BatchUpdateDto request)
    {
        var result = await structureServices.UpdateStructureAsync(request);

        if (result.IsSuccess)
            return Ok(new SuccessDto("Template structure updated successfully."));

        return BuildBatchErrorResponse(result.Errors);
    }

    private UnprocessableEntityObjectResult BuildBatchErrorResponse(IEnumerable<IError> errors)
    {
        var allErrors = errors
            .Select(e => new ErrorDto(e.Message, e.GetCode(), e.Metadata))
            .ToList();

        var response = new StructureErrorDto("Structure validation failed.", allErrors);

        return UnprocessableEntity(response);
    }
}