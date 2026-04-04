using System.Linq;

using Arpg.Client.Core;

using Arpg.Primitives.Constants;

using FluentResults;
using FluentValidation.Results;

namespace Arpg.Client.Extensions;

public static class ValidationExtensions
{
    extension(ValidationResult validationResult)
    {
        public Result ToApiError()
        {
            var errors = validationResult.Errors.Select(e =>
            {
                var error = new ApiError(e.ErrorMessage, e.ErrorCode, []);

                error.WithMetadata(MetadataKey.Error, e.ErrorCode);

                if (e.FormattedMessagePlaceholderValues is null) return error;
                
                foreach (var placeholder in e.FormattedMessagePlaceholderValues)   
                    error.WithMetadata(placeholder.Key, placeholder.Value);

                return error;
            });

            return Result.Fail(errors);
        }
    }
}