using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Arpg.Client.Abstractions;
using Arpg.Client.Core;
using Arpg.Client.Extensions;
using Arpg.Contracts.Dto.Template;
using Arpg.Primitives.Codes;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Arpg.Client.Services;

public class TemplateServices(HttpClient client, ILogger<TemplateServices> logger) : ITemplateServices
{
    public async Task<Result<List<SimpleTemplateDto>>> GetListAsync()
    {
        try
        {
            var response = await client.GetAsync(ApiEndpoints.Template.GetList);
            return await response.ToResultAsync<List<SimpleTemplateDto>>();
        }
        catch (HttpRequestException)
        {
            logger.LogError("No connection to the server.");
            return Result.Fail(new ApiError("No connection to the server.", GeneralCodes.NoConnection, []));
        }
        catch (Exception ex)
        {
            logger.LogError("An unknown error occurred. {ex}", ex.Message);
            return Result.Fail(new ApiError("Internal Fail.", GeneralCodes.Domain, []));
        }
    }

    public async Task<Result<SuccessCreateDto>> CreateAsync(NewTemplateDto dto)
    {
        try
        {
            var response = await client.PostAsJsonAsync(ApiEndpoints.Template.Create, dto, AppJsonContext.Default.NewTemplateDto);
            return await response.ToResultAsync<SuccessCreateDto>();
        }
        catch (HttpRequestException)
        {
            logger.LogError("No connection to the server.");
            return Result.Fail(new ApiError("No connection to the server.", GeneralCodes.NoConnection, []));
        }
        catch (Exception ex)
        {
            logger.LogError("An unknown error occurred. {ex}", ex.Message);
            return Result.Fail(new ApiError("Internal Fail.", GeneralCodes.Domain, []));
        }
    }

    public async Task<Result> DeleteAsync(DeleteTemplateDto dto)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, ApiEndpoints.Template.Delete)
            {
                Content = JsonContent.Create(dto, AppJsonContext.Default.DeleteTemplateDto)
            };
            var response = await client.SendAsync(request);
            return await response.ToResultAsync();
        }
        catch (HttpRequestException)
        {
            logger.LogError("No connection to the server.");
            return Result.Fail(new ApiError("No connection to the server.", GeneralCodes.NoConnection, []));
        }
        catch (Exception ex)
        {
            logger.LogError("An unknown error occurred. {ex}", ex.Message);
            return Result.Fail(new ApiError("Internal Fail.", GeneralCodes.Domain, []));
        }
    }
}
