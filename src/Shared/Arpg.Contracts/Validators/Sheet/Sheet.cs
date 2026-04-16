using Arpg.Contracts.Dto.Sheet;
using Arpg.Primitives.Codes;
using FluentValidation;

namespace Arpg.Contracts.Validators.Sheet;

public class CreateSheetValidator : AbstractValidator<CreateDto>
{
    public CreateSheetValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithErrorCode(DataFormatCodes.Required)
            .WithMessage("Sheet name is required.")

            .Length(2, 50)
            .WithErrorCode(DataFormatCodes.InvalidSize)
            .WithMessage("The username must be at least 2 characters long.");

        RuleFor(x => x.TemplateId)
            .NotEmpty()
            .WithErrorCode(DataFormatCodes.Required)
            .WithMessage("Template id is required.");
    }
}

public class EditSheetValidator : AbstractValidator<EditDto>
{
    public EditSheetValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithErrorCode(DataFormatCodes.Required)
            .WithMessage("Sheet id is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithErrorCode(DataFormatCodes.Required)
            .WithMessage("Sheet name is required.")

            .Length(2, 50)
            .WithErrorCode(DataFormatCodes.InvalidSize)
            .WithMessage("The username must be at least 2 characters long."); ;
    }
}

public class ComputeSheetValidator : AbstractValidator<ComputeDto>
{
    public ComputeSheetValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithErrorCode(DataFormatCodes.Required)
            .WithMessage("Sheet id is required.");

        RuleFor(x => x.Data)
            .NotNull()
            .WithErrorCode(DataFormatCodes.Required)
            .WithMessage("Data is required.");
    }
}