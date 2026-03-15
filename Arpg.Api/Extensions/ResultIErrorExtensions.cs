using Arpg.Shared.Codes;
using Arpg.Shared.Constants;
using Arpg.Shared.Results;
using FluentResults;

namespace Arpg.Api.Extensions;

public static class ResultIErrorExtensions
{
    extension(IError error)
    {
        public string GetCode()
        {
            error.Metadata.TryGetValue(MetadataKey.Error, out var obj);

            return obj as string ?? error switch
            {
                NotFoundError => GeneralCodes.NotFound,
                ConflictError => GeneralCodes.Conflict,
                _ => GeneralCodes.Generic
            };
        }
    }
}