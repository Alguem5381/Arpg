using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Arpg.Client.Abstractions;
using Arpg.Client.Core;
using Arpg.Client.Extensions;
using Arpg.Contracts.Dto.User;
using Arpg.Primitives.Codes;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Arpg.Client.Services;

public class AuthServices
(
    HttpClient client, 
    IUserSession userSession,
    IValidator<LoginDto> loginDtoValidator,
    IValidator<NewDto> newDtoValidator,
    IValidator<ValidateCodeDto> validateCodeDtoValidator,
    ILogger<AuthServices> logger
) : IAuthServices
{
    public async Task<Result<CodeDto>> LoginAsync(LoginDto request)
    {
        var validation = await loginDtoValidator.ValidateAsync(request);

        if (!validation.IsValid)
            return validation.ToApiError();
        
        try
        {
            var response =
                await client.PostAsJsonAsync(ApiEndpoints.User.Login, request, AppJsonContext.Default.LoginDto);

            var result = await response.ToResultAsync<CodeDto>();

            if (result.IsFailed)
                return Result.Fail(result.Errors);

            logger.LogInformation("Login Initialized. Waiting for 2FA validation.");
            return Result.Ok(result.Value);
        }
        catch (HttpRequestException)
        {
            logger.LogError("No connection to the server.");
            return Result.Fail(new ApiError("No connection to the server.", GeneralCodes.NoConnection, []));
        }
        catch (TaskCanceledException)
        {
            logger.LogError("The request took a long time.");
            return Result.Fail(new ApiError("The request took a long time.", GeneralCodes.Timeout, []));
        }
        catch (Exception ex)
        {
            logger.LogError("An unknown error occurred. {ex}", ex.Message);
            return Result.Fail(new ApiError("Internal Fail.", GeneralCodes.Domain, []));
        }
    }

    public async Task<Result<CodeDto>> NewAsync(NewDto request)
    {
        var validation = await newDtoValidator.ValidateAsync(request);

        if (!validation.IsValid)
            return validation.ToApiError();

        try
        {
            var response =
                await client.PostAsJsonAsync(ApiEndpoints.User.New, request, AppJsonContext.Default.NewDto);

            var result = await response.ToResultAsync<CodeDto>();

            if (result.IsFailed)
                return Result.Fail(result.Errors);

            logger.LogInformation("Registration Initialized. Waiting for 2FA validation.");
            return Result.Ok(result.Value);
        }
        catch (HttpRequestException)
        {
            logger.LogError("No connection to the server.");
            return Result.Fail(new ApiError("No connection to the server.", DataFormatCodes.ValidationError, []));
        }
        catch (TaskCanceledException)
        {
            logger.LogError("The request took a long time.");
            return Result.Fail(new ApiError("The request took a long time.", GeneralCodes.Timeout, []));
        }
        catch (Exception ex)
        {
            logger.LogError("An unknown error occurred. {ex}", ex.Message);
            return Result.Fail(new ApiError("Internal Fail.", GeneralCodes.Domain, []));
        }
    }

    public async Task<Result> ValidateAsync(ValidateCodeDto request)
    {
        var validation = await validateCodeDtoValidator.ValidateAsync(request);

        if (!validation.IsValid)
            return validation.ToApiError();
            
        try
        {
            var response =
                await client.PostAsJsonAsync(ApiEndpoints.User.Validate, request, AppJsonContext.Default.ValidateCodeDto);

            var result = await response.ToResultAsync<SuccessLoginDto>();

            if (result.IsFailed)
                return Result.Fail(result.Errors);

            userSession.Token = result.Value.Token;

            logger.LogInformation("2FA Validation successful. Logged in.");
            return Result.Ok();
        }
        catch (HttpRequestException)
        {
            logger.LogError("No connection to the server.");
            return Result.Fail(new ApiError("No connection to the server.", GeneralCodes.NoConnection, []));
        }
        catch (TaskCanceledException)
        {
            logger.LogError("The request took a long time.");
            return Result.Fail(new ApiError("The request took a long time.", GeneralCodes.Timeout, []));
        }
        catch (Exception ex)
        {
            logger.LogError("An unknown error occurred. {ex}", ex.Message);
            return Result.Fail(new ApiError("Internal Fail.", GeneralCodes.Domain, []));
        }
    }
}
