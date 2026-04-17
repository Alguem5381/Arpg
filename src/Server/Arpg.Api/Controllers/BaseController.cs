using Arpg.Contracts.Dto.General;
using Arpg.Primitives.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Arpg.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BaseController : ControllerBase
{
    protected IActionResult ToFailResults(ResultBase result, bool isUnprocessableEntity = false)
    {
        var response =
            new ErrorResponseDto(result.Errors.Select(e => new ErrorDto(e.Message, e.GetCode(), e.Metadata)).ToList());

        if (result.Errors.Count > 1)
            return isUnprocessableEntity ? UnprocessableEntity(response) : BadRequest(response);

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