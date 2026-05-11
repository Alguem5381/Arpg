using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Arpg.Client.Core;
using Arpg.Primitives.Codes;
using FluentResults;
using Serilog;

namespace Arpg.Client.Extensions;

public static class HttpResponseExtensions
{
    extension(HttpResponseMessage response)
    {
        public async Task<Result<T>> ToResultAsync<T>()
        {
            if (AppJsonContext.Default.GetTypeInfo(typeof(T)) is not JsonTypeInfo<T> typeInfoT)
                throw new InvalidOperationException(
                    $"The type {typeof(T).Name} was not registered in the AppJsonContext.");

            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    return Result.Ok(default(T)!);

                var data = await response.Content.ReadFromJsonAsync(typeInfoT);
                return Result.Ok(data!);
            }

            var rawBody = await response.Content.ReadAsStringAsync();
            var result = new Result<T>();

            try
            {
                var errorResponseDto = JsonSerializer.Deserialize(rawBody, AppJsonContext.Default.ErrorResponseDto);
                if (errorResponseDto is not null && errorResponseDto.Errors.Count != 0)
                {
                    foreach (var e in errorResponseDto.Errors)
                        Log.Warning("API [{StatusCode}] {Code} — {Error} | Metadata: {Metadata}",
                            (int)response.StatusCode, e.Code, e.Error,
                            string.Join(", ", e.Metadata.Select(m => $"{m.Key}={m.Value}")));

                    return result.WithErrors(
                        errorResponseDto.Errors.Select(e => new ApiError(e.Error, e.Code, e.Metadata)));
                }
            }
            catch (Exception e)
            {
                Log.Warning("Fail parser: {EMessage}", e.Message);
                Log.Warning("API error {StatusCode} — raw body: {Body}", (int)response.StatusCode, rawBody);
            }

            var genericCode = response.StatusCode switch
            {
                System.Net.HttpStatusCode.NotFound => GeneralCodes.NotFound,
                System.Net.HttpStatusCode.Unauthorized => GeneralCodes.Unauthorized,
                _ => GeneralCodes.Generic
            };

            return result.WithError(new ApiError("Erro na comunicação com a API.", genericCode, []));
        }

        public async Task<Result> ToResultAsync()
        {
            if (response.IsSuccessStatusCode)
                return Result.Ok();

            var rawBody = await response.Content.ReadAsStringAsync();
            var result = new Result();

            try
            {
                var errorResponseDto = JsonSerializer.Deserialize(rawBody, AppJsonContext.Default.ErrorResponseDto);
                if (errorResponseDto is not null && errorResponseDto.Errors.Count != 0)
                {
                    foreach (var e in errorResponseDto.Errors)
                        Log.Warning("API [{StatusCode}] {Code} — {Error} | Metadata: {Metadata}",
                            (int)response.StatusCode, e.Code, e.Error,
                            string.Join(", ", e.Metadata.Select(m => $"{m.Key}={m.Value}")));

                    return result.WithErrors(
                        errorResponseDto.Errors.Select(e => new ApiError(e.Error, e.Code, e.Metadata)));
                }
            }
            catch (Exception e)
            {
                Log.Warning("Fail parser: {EMessage}", e.Message);
                Log.Warning("API error {StatusCode} — raw body: {Body}", (int)response.StatusCode, rawBody);
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