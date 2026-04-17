using Arpg.Contracts.Dto.GameTable;
using Arpg.Primitives.Codes;
using FluentValidation;

namespace Arpg.Contracts.Validators.GameTable;

public class NewGameTableValidator : AbstractValidator<NewDto>
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

public class PlayerOperationValidator : AbstractValidator<GameTableOperationDto>
{
    public PlayerOperationValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("PlayerId is required.")
            .WithErrorCode(DataFormatCodes.Required);

        RuleFor(x => x.Operation)
            .NotEmpty()
            .WithMessage("Op is required.")
            .WithErrorCode(DataFormatCodes.Required);
    }
}
