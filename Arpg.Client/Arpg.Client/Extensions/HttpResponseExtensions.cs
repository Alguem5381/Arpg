using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization.Metadata;

using Arpg.Client.Core;
using Arpg.Shared.Codes;
using FluentResults;

namespace Arpg.Client.Extensions;

public static class HttpResponseExtensions
{
    extension(HttpResponseMessage response)
    {
        public async Task<Result<T>> ToResultAsync<T>()
        {
            var typeInfoError = AppJsonContext.Default.ErrorResponseDto;

            if (AppJsonContext.Default.GetTypeInfo(typeof(T)) is not JsonTypeInfo<T> typeInfoT)
                throw new InvalidOperationException($"The type {{typeof(T).Name}} was not registered in the AppJsonContext.");

            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    return Result.Ok(default(T)!);

                var data = await response.Content.ReadFromJsonAsync(typeInfoT);
                return Result.Ok(data!);
            }

            var result = new Result<T>();

            try
            {
                var errorResponseDto = await response.Content.ReadFromJsonAsync(typeInfoError);
                if (errorResponseDto is not null && errorResponseDto.Errors.Count != 0)
                    return result.WithErrors(errorResponseDto.Errors.Select(e => new ApiError(e.Error, e.Code, e.Metadata)));
            }
            catch
            {
                // ignored
            }

            var genericCode = response.StatusCode switch
            {
                System.Net.HttpStatusCode.NotFound => GeneralCodes.NotFound,
                System.Net.HttpStatusCode.Unauthorized => GeneralCodes.Unauthorized,
                _ => GeneralCodes.Generic
            };

            return result.WithError(new ApiError("Erro na comunicação com a API.", genericCode, []));
        }
    }
}