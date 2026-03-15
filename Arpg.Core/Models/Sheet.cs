using Arpg.Core.Helpers;

using Arpg.Shared.Codes;
using Arpg.Shared.Constants;
using Arpg.Shared.Results;

using FluentResults;

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
                .WithMetadata(MetadataKey.Error, TemplateCodes.InvalidTemplate));
        
        foreach(var field in structure.Fields)
            Data[field.Id] = field.DefaultValue;

        return Result.Ok();
    }

    public Result<Dictionary<Guid, object?>> ComputeData(Dictionary<Guid, object?> inputValues, TemplateStructure template)
    {
        Dictionary<Guid, object?> newData = [];
        
        List<IError> errors = [];

        foreach (var field in template.Fields)
        {
            object? valueToProcess;

            if (inputValues.TryGetValue(field.Id, out var value) || Data.TryGetValue(field.Id, out value))
                valueToProcess = value;
            else
                valueToProcess = field.DefaultValue;

            if (!TypeChecker.IsValueValidForType(valueToProcess, field.Type))
            {
                errors.Add(new InvalidFieldError($"Value for '{field.Name}' is invalid type.", field.Id, field.CategoryId)
                    .WithMetadata(MetadataKey.Error, StructureCodes.FieldAndTypeMismatch));
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
