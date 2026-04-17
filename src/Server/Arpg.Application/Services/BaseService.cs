using Arpg.Primitives.Constants;
using Arpg.Primitives.Results;
using FluentValidation;

namespace Arpg.Application.Services;

public class BaseService
{
    protected static Result Validate<T>(IValidator<T> validator, T model)
    {
        ArgumentNullException.ThrowIfNull(validator);

        var validationResult = validator.Validate(model);

        if (validationResult.IsValid)
            return Result.Ok();

        var errors = validationResult.Errors.Select(e =>
        {
            var error = new ValidationError(e.ErrorMessage)
                .WithMetadata(MetadataKey.Error, e.ErrorCode);

            if (e.FormattedMessagePlaceholderValues is null) return error;

            foreach (var (key, value) in e.FormattedMessagePlaceholderValues)
                error.WithMetadata(key, value);

            return error;
        });

        return Result.Fail(errors);
    }

    protected static Result Validate<T>(IValidator<T> validator, IEnumerable<T> models)
    {
        ArgumentNullException.ThrowIfNull(validator);

        var errors = new List<ValidationError>();

        foreach (var model in models)
        {
            var validationResult = validator.Validate(model);

            if (validationResult.IsValid)
                continue;

            errors.AddRange(validationResult.Errors.Select(e =>
            {
                var error = new ValidationError(e.ErrorMessage);

                error.WithMetadata(MetadataKey.Error, e.ErrorCode);

                if (e.FormattedMessagePlaceholderValues is null) return error;

                foreach (var placeholder in e.FormattedMessagePlaceholderValues)
                    error.WithMetadata(placeholder.Key, placeholder.Value);

                return error;
            }));
        }

        return errors.Count == 0 ? Result.Ok() : Result.Fail(errors);
    }
}