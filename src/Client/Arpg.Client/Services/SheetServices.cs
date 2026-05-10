using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Arpg.Client.Abstractions;
using Arpg.Client.Core;
using Arpg.Client.Extensions;
using Arpg.Contracts.Dto.Sheet;
using Arpg.Primitives.Codes;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Arpg.Client.Services;

public class SheetServices(HttpClient client, ILogger<SheetServices> logger) : ISheetServices
{
    public async Task<Result<List<SimpleSheetDto>>> GetListAsync()
    {
        try
        {
            var response = await client.GetAsync(ApiEndpoints.Sheet.GetList);
            return await response.ToResultAsync<List<SimpleSheetDto>>();
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
