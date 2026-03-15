using Arpg.Application.Auth;
using Arpg.Application.Services;

using Arpg.Contracts.Dto.General;
using Arpg.Contracts.Dto.User;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Arpg.Api.Controllers;

public class UserController(UserServices userServices, AccountServices accountServices) : BaseController
{
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> New([FromBody] NewDto request)
    {
        var result = await accountServices.NewAsync(request);

        return result.IsFailed 
            ? ToFailResults(result) 
            : CreatedAtAction
            (
                nameof(GetFlat),
                new { username = request.Username },
                result.Value
            );
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        var result = await accountServices.LoginAsync(request);

        return result.IsFailed ? ToFailResults(result) : Ok(result.Value);
    }

    [HttpPost("validate")]
    [AllowAnonymous]
    public async Task<IActionResult> Validate([FromBody] ValidateCodeDto request)
    {
        var result = await accountServices.ValidateCode(request.Key, request.Value);

        return result.IsFailed ? ToFailResults(result) : Ok(new SuccessLoginDto("Logged in successfully.", result.Value));
    }

    [HttpGet("{username}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetFlat(string username)
    {
        var result = await userServices.GetFlatAsync(username);

        return result.IsFailed ? ToFailResults(result) : Ok(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> GetSelf()
    {
        var result = await userServices.GetSelfAsync();

        return result.IsFailed ? ToFailResults(result) : Ok(result.Value);
    }

    [HttpPut]
    public async Task<IActionResult> Edit([FromBody] EditDto request)
    {
        var result = await userServices.EditAsync(request);

        return result.IsFailed ? ToFailResults(result) : Ok(new SuccessDto($"Name change successfully to {result.Value.DisplayName}."));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteDto request)
    {
        var result = await userServices.DeleteAsync(request);

        return result.IsFailed ? ToFailResults(result) : Ok(new SuccessDto("User deleted successfully."));
    }
}