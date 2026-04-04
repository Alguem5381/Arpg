using Arpg.Primitives.Constants;
using Arpg.Primitives.Results;
using FluentResults;
using FluentValidation.Results;

namespace Arpg.Application.Extensions;

public static class ValidationExtensions
{
    extension(ValidationResult validationResult)
    {
        public Result ToResult()
        {
            var errors = validationResult.Errors.Select(e =>
            {
                var error = new ValidationError(e.ErrorMessage);

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