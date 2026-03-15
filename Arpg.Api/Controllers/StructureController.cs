using Arpg.Application.Services;
using Arpg.Contracts.Dto.General;
using Arpg.Contracts.Dto.Structure;
using Arpg.Shared.Codes;
using Arpg.Shared.Results;
using Arpg.Api.Extensions;
using Arpg.Shared.Constants;

using FluentResults;

using Microsoft.AspNetCore.Mvc;

namespace Arpg.Api.Controllers;

public class StructureController(StructureServices structureServices) : BaseController
{
    [HttpPut("{templateId:guid}/structure")]
    public async Task<IActionResult> UpdateStructure(Guid templateId, [FromBody] BatchUpdateDto request)
    {
        if (request.TemplateId == Guid.Empty) 
            request.TemplateId = templateId;

        if (request.TemplateId != templateId)
            return ToFailResults(Result.Fail(new NotFoundError("Route ID mismatch with Body ID")
                .WithMetadata(MetadataKey.Error, TemplateCodes.TemplateMismatch)));

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