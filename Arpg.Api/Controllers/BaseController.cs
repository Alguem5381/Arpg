using Arpg.Api.Extensions;
using Arpg.Shared.Codes;
using Arpg.Shared.Constants;
using Arpg.Shared.Results;

using Arpg.Contracts.Dto.General;

using FluentResults;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Arpg.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BaseController : ControllerBase
{
    protected IActionResult ToFailResults(ResultBase result)
    {
        var response = new ErrorResponseDto([.. result.Errors.Select(e => new ErrorDto(e.Message, e.GetCode(), e.Metadata))]);

        if (result.Errors.Count > 1)
            return BadRequest(response);

        var firstError = result.Errors[0];

        return firstError switch
        {
            NotFoundError or ForbiddenError => NotFound(response),
            ConflictError => Conflict(response),
            ValidationError or UnprocessableEntityError => UnprocessableEntity(response),
            UnauthorizedError => Unauthorized(response),
            _ => BadRequest(response)
        };
    }
}