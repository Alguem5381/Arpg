using Arpg.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Arpg.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpressionResolverController(ExpressionResolver resolver) : ControllerBase
{
    [HttpPost]
    public IActionResult Resolver([FromBody] Roll roll)
    {
        try
        {
            string? result = resolver.Resolve(roll.Formula);

            if (result == null)
                return BadRequest(new { Error = "Invalid formula." });

            return Ok(new
            {
                ReceivedFormula = roll.Formula,
                FinalResult = result,
                Message = "Success."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Erro = ex.Message });
        }
    }
}

public record Roll(string Formula);