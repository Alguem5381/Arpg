using Arpg.Primitives.Codes;
using Arpg.Primitives.Constants;
using Arpg.Primitives.Results;
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