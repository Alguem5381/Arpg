using Arpg.Primitives.Codes;

using Arpg.Contracts.Dto.General;

using Microsoft.AspNetCore.Diagnostics;

namespace Arpg.Api.Controllers;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = 500;
        httpContext.Response.ContentType = "application/json";

        Console.WriteLine(exception.ToString());

        var response = new ErrorResponseDto([new ErrorDto("Internal Fail", GeneralCodes.Domain, [])]);

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}