using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Arpg.Client.Abstractions;
using Arpg.Client.Core;
using Arpg.Client.Extensions;
using Arpg.Contracts.Dto.User;
using Arpg.Shared.Codes;
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
    ILogger<AuthServices> logger
) : IAuthServices
{
    public async Task<Result> LoginAsync(LoginDto request)
    {
        var validation = await loginDtoValidator.ValidateAsync(request);

        if (!validation.IsValid)
            return validation.ToApiError();
        
        try
        {
            var response =
                await client.PostAsJsonAsync(ApiEndpoints.User.Login, request, AppJsonContext.Default.LoginDto);

            var result = await response.ToResultAsync<SuccessLoginDto>();

            if (result.IsFailed)
                return Result.Fail(result.Errors);

            userSession.Token = result.Value.Token;

            logger.LogInformation("Login successful");
            return Result.Ok();
        }
        catch (HttpRequestException ex)
        {
            logger.LogError("No connection to the server.");
            return Result.Fail(new ApiError("No connection to the server.", GeneralCodes.NoConnection, []));
        }
        catch (TaskCanceledException ex)
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

    public async Task<Result> New(NewDto request)
    {
        var validation = await newDtoValidator.ValidateAsync(request);

        if (!validation.IsValid)
            return validation.ToApiError();

        try
        {
            var response =
                await client.PostAsJsonAsync(ApiEndpoints.User.New, request, AppJsonContext.Default.NewDto);

            var result = await response.ToResultAsync<SuccessLoginDto>();

            if (result.IsFailed)
                return Result.Fail(result.Errors);

            userSession.Token = result.Value.Token;

            logger.LogInformation("Login successful");
            return Result.Ok();
        }
        catch (HttpRequestException ex)
        {
            logger.LogError("No connection to the server.");
            return Result.Fail(new ApiError("No connection to the server.", DataFormatCodes.ValidationError, []));
        }
        catch (TaskCanceledException ex)
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
