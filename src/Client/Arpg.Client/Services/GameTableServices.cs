using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Arpg.Client.Abstractions;
using Arpg.Client.Core;
using Arpg.Client.Extensions;
using Arpg.Contracts.Dto.GameTable;
using Arpg.Primitives.Codes;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Arpg.Client.Services;

public class GameTableServices(HttpClient client, ILogger<GameTableServices> logger) : ITableServices
{
    public async Task<Result<List<SimpleGameTableDto>>> GetListAsync()
    {
        try
        {
            var response = await client.GetAsync(ApiEndpoints.GameTable.GetList);
            return await response.ToResultAsync<List<SimpleGameTableDto>>();
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
