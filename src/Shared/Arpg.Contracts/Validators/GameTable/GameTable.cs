using Arpg.Contracts.Dto.GameTable;
using Arpg.Primitives.Codes;
using FluentValidation;

namespace Arpg.Contracts.Validators.GameTable;

public class NewGameTableValidator : AbstractValidator<NewTableDto>
{
    public NewGameTableValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .WithErrorCode(DataFormatCodes.Required)
            .Length(2, 50)
            .WithMessage("The username must be at least 2 characters long.")
            .WithErrorCode(DataFormatCodes.InvalidSize);

        RuleFor(x => x.TemplateId)
            .NotEmpty()
            .WithMessage("Template ID is required.")
            .WithErrorCode(DataFormatCodes.Required);
    }
}

public class OperationValidator : AbstractValidator<GameTableOperationDto>
{
    public OperationValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("PlayerId is required.")
            .WithErrorCode(DataFormatCodes.Required);

        RuleFor(x => x.Operation)
            .NotNull()
            .WithMessage("Operation is required.")
            .WithErrorCode(DataFormatCodes.Required);
    }
}
