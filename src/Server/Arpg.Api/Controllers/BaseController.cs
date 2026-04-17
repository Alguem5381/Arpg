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
    protected IActionResult ToFailResults(ResultBase result)
    {
        var response =
            new ErrorResponseDto(result.Errors.Select(e => new ErrorDto(e.Message, e.GetCode(), e.Metadata)).ToList());

        if (result.Errors.Count > 1)
        {
            var allUnprocessable = result.Errors.All(e => e is ValidationError or UnprocessableEntityError);
            return allUnprocessable ? UnprocessableEntity(response) : BadRequest(response);
        }

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