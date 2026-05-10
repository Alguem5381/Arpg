using Arpg.Core.Helpers;
using Arpg.Core.Models.Definitions;
using Arpg.Primitives.Codes;
using Arpg.Primitives.Constants;
using Arpg.Primitives.Results;

namespace Arpg.Core.Models;

public class Sheet
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OwnerId { get; set; }
    public Guid TemplateId { get; init; }
    public string Name { get; set; } = string.Empty;
    public Dictionary<Guid, object?> Data { get; private set; } = [];
    public DateTime CreateAt { get; } = DateTime.UtcNow;

    public Result Populate(TemplateStructure structure)
    {
        if (structure.Fields.Count == 0)
            return Result.Fail(new ValidationError("Template is invalid.")
                .With(Key.Error, TemplateCodes.InvalidTemplate));

        foreach (var field in structure.Fields)
            Data[field.Id] = field.DefaultValue;

        return Result.Ok();
    }

    public Result<Dictionary<Guid, object?>> ComputeData(Dictionary<Guid, object?> inputValues,
        TemplateStructure template)
    {
        Dictionary<Guid, object?> newData = [];

        List<Error> errors = [];

        foreach (var field in template.Fields)
        {
            object? valueToProcess;

            if (inputValues.TryGetValue(field.Id, out var value) || Data.TryGetValue(field.Id, out value))
                valueToProcess = value;
            else
                valueToProcess = field.DefaultValue;

            if (!TypeChecker.IsValueValidForType(valueToProcess, field.Type))
            {
                errors.Add(new ValidationError($"Value for '{field.Name}' is invalid type.")
                    .With(Key.Error, StructureCodes.FieldAndTypeMismatch)
                    .With(Key.Category, field.CategoryId)
                    .With(Key.Field, field.Id));
                continue;
            }

            if (valueToProcess == null && field.IsRequired)
                newData[field.Id] = field.DefaultValue;
            else
                newData[field.Id] = valueToProcess;
        }

        Data = newData;

        return errors.Count > 0 ? Result.Fail(errors) : Result.Ok(newData);
    }
}
