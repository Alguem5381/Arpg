using Arpg.Core.Extensions;
using Arpg.Core.Helpers;
using Arpg.Core.Models.Definitions.Batch;
using Arpg.Primitives.Codes;
using Arpg.Primitives.Constants;
using Arpg.Primitives.Enums.Template;
using Arpg.Primitives.Results;
using FluentResults;

namespace Arpg.Core.Models.Definitions;

public class TemplateStructure
{
    public List<TemplateCategory> Categories { get; set; } = [];
    public List<TemplateField> Fields { get; set; } = [];

    public Result Update(BatchUpdate batch)
    {
        List<IError> errors = [];

        //Itera sobre as operações de categorias e processa cada uma
        errors.AddNullableRange(batch.Categories?.SelectMany(catOp => ProcessCategory(this, catOp).Errors));

        //Itera sobre as operações de campos e processa cada uma
        errors.AddNullableRange(batch.Fields?.SelectMany(fieldOp => ProcessField(this, fieldOp).Errors));

        //Verifica se há campos órfãos
        var validCategoryIds = Categories.Select(c => c.Id).ToHashSet();

        errors.AddRange(Fields
            .Where(f => !validCategoryIds.Contains(f.CategoryId))
            .Select(f =>
                new ValidationError($"Field '{f.Name}' references a missing Category ({f.CategoryId}).")
                    .WithMetadata(MetadataKey.Error, StructureCodes.OrphanField)
                    .WithMetadata(MetadataKey.Category, f.CategoryId)
                    .WithMetadata(MetadataKey.Field, f.Id)));

        return errors.Count > 0 ? Result.Fail(errors) : Result.Ok();

        static Result ProcessCategory(TemplateStructure structure, CategoryOperation op)
        {
            return op.Op switch
            {
                Operation.Add => AddCategory(structure, op),
                Operation.Edit => EditCategory(structure, op),
                Operation.Delete => DeleteCategory(structure, op),
                _ => Result.Fail(new Error("Invalid operation")
                    .WithMetadata(MetadataKey.Error, StructureCodes.InvalidStructureOperation))
            };
        }

        static Result AddCategory(TemplateStructure structure, CategoryOperation op)
        {
            if (structure.Categories.Any(c => c.Id == op.Id))
                return Result.Fail(new ValidationError("Category ID conflict.")
                    .WithMetadata(MetadataKey.Error, StructureCodes.CategoryConflict)
                    .WithMetadata(MetadataKey.Category, op.Id));

            structure.Categories.Add(new TemplateCategory
            {
                Id = op.Id,
                Name = op.Name ?? "New Category",
                Order = op.Order ?? structure.Categories.Count
            });
            return Result.Ok();
        }

        static Result EditCategory(TemplateStructure structure, CategoryOperation op)
        {
            var cat = structure.Categories.FirstOrDefault(c => c.Id == op.Id);
            if (cat == null)
                return Result.Fail(new ValidationError("Category not found.")
                    .WithMetadata(MetadataKey.Error, StructureCodes.CategoryNotFound)
                    .WithMetadata(MetadataKey.Category, op.Id));

            if (op.Name != null) cat.Name = op.Name;
            if (op.Order != null) cat.Order = op.Order.Value;

            return Result.Ok();
        }

        static Result DeleteCategory(TemplateStructure structure, CategoryOperation op)
        {
            var cat = structure.Categories.FirstOrDefault(c => c.Id == op.Id);
            if (cat == null) return Result.Fail(new ValidationError("Category not found.")
                .WithMetadata(MetadataKey.Error, StructureCodes.CategoryNotFound)
                .WithMetadata(MetadataKey.Category, op.Id));

            structure.Categories.Remove(cat);
            return Result.Ok();
        }

        static Result ProcessField(TemplateStructure structure, FieldOperation op)
        {
            return op.Op switch
            {
                Operation.Add => AddField(structure, op),
                Operation.Edit => EditField(structure, op),
                Operation.Delete => DeleteField(structure, op),
                _ => Result.Fail(new Error("Invalid operation")
                    .WithMetadata(MetadataKey.Error, StructureCodes.InvalidStructureOperation))
            };
        }

        static Result AddField(TemplateStructure structure, FieldOperation op)
        {
            if (structure.Fields.Any(f => f.Id == op.Id))
                return Result.Fail(new ValidationError("Field ID conflict.")
                    .WithMetadata(MetadataKey.Error, StructureCodes.FieldIdConflict)
                    .WithMetadata(MetadataKey.Category, op.CategoryId)
                    .WithMetadata(MetadataKey.Field, op.Id));

            if (!TypeChecker.IsValueValidForType(op.DefaultValue, op.Type ?? FieldType.Text))
                return Result.Fail(new ValidationError("Default value does not match field type.")
                    .WithMetadata(MetadataKey.Error, StructureCodes.FieldAndTypeMismatch)
                    .WithMetadata(MetadataKey.Category, op.CategoryId)
                    .WithMetadata(MetadataKey.Field, op.Id));

            structure.Fields.Add(new TemplateField
            {
                Id = op.Id,
                CategoryId = op.CategoryId,
                Name = op.Name ?? "New Field",
                Type = op.Type ?? FieldType.Text,
                DefaultValue = op.DefaultValue,
                IsRequired = op.IsRequired ?? false
            });

            return Result.Ok();
        }

        static Result EditField(TemplateStructure structure, FieldOperation op)
        {
            var field = structure.Fields.FirstOrDefault(f => f.Id == op.Id);

            if (field == null)
                return Result.Fail(new ValidationError("Field not found.")
                    .WithMetadata(MetadataKey.Error, StructureCodes.FieldNotFound)
                    .WithMetadata(MetadataKey.Category, op.CategoryId)
                    .WithMetadata(MetadataKey.Field, op.Id));

            var typeToCheck = op.Type ?? field.Type;
            object? valueToCheck;

            if (op.SetDefaultValueToNull)
                valueToCheck = null;
            else
                valueToCheck = op.DefaultValue ?? field.DefaultValue;

            if (!TypeChecker.IsValueValidForType(valueToCheck, typeToCheck))
                return Result.Fail(new ValidationError("Value incompatible with Type.")
                    .WithMetadata(MetadataKey.Error, StructureCodes.FieldAndTypeMismatch)
                    .WithMetadata(MetadataKey.Category, op.CategoryId)
                    .WithMetadata(MetadataKey.Field, op.Id));

            if (op.Name != null) field.Name = op.Name;
            if (op.CategoryId != Guid.Empty) field.CategoryId = op.CategoryId;
            if (op.IsRequired != null) field.IsRequired = op.IsRequired.Value;
            if (op.Type != null) field.Type = op.Type.Value;
            if (op.SetDefaultValueToNull) field.DefaultValue = null;
            else if (op.DefaultValue != null) field.DefaultValue = op.DefaultValue;

            return Result.Ok();
        }

        static Result DeleteField(TemplateStructure structure, FieldOperation op)
        {
            var field = structure.Fields.FirstOrDefault(f => f.Id == op.Id);

            if (field == null) return Result.Fail(new ValidationError("Field not found.")
                .WithMetadata(MetadataKey.Error, StructureCodes.FieldNotFound)
                .WithMetadata(MetadataKey.Category, op.CategoryId)
                .WithMetadata(MetadataKey.Field, op.Id));

            structure.Fields.Remove(field);
            return Result.Ok();
        }
    }
}

public class TemplateCategory
{
    public Guid Id { get; init; } = Guid.Empty;
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
}

public class TemplateField
{
    public Guid Id { get; init; } = Guid.Empty;
    public Guid CategoryId { get; set; } = Guid.Empty;
    public string Name { get; set; } = string.Empty;
    public FieldType Type { get; set; }
    public object? DefaultValue { get; set; }
    public bool IsRequired { get; set; }
}