using Arpg.Contracts.Dto.Template;
using Arpg.Shared.Codes;
using FluentValidation;

namespace Arpg.Contracts.Validators.Template;

public class CreateDtoValidator : AbstractValidator<TemplateCreateDto>
{
    public CreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("The name is required")
            .WithErrorCode(nameof(DataFormatCodes.Required))

            .Length(2, 50)
            .WithMessage("The name must be at least 2 characters long.")
            .WithErrorCode(nameof(DataFormatCodes.InvalidSize));
    }
}

public class EditDtoValidator : AbstractValidator<TemplateEditDto>
{
    public EditDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("A id is required")
            .WithErrorCode(nameof(DataFormatCodes.Required));

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("The name is required")
            .WithErrorCode(nameof(DataFormatCodes.Required))

            .Length(2, 50)
            .WithMessage("The name must be at least 2 characters long.")
            .WithErrorCode(nameof(DataFormatCodes.InvalidSize));

        RuleFor(x => x.Description)
            .Length(2, 50)
            .WithMessage("The description must be at least 2 characters long.")
            .WithErrorCode(nameof(DataFormatCodes.InvalidSize));
    }
}

public class DeleteDtoValidator : AbstractValidator<TemplateDeleteDto>
{
    public DeleteDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("A id is required")
            .WithErrorCode(nameof(DataFormatCodes.Required));

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("A password is required")
            .WithErrorCode(nameof(DataFormatCodes.Required))

            .Length(8, 50)
            .WithMessage("The password must be at least 8 characters long.")
            .WithErrorCode(nameof(DataFormatCodes.InvalidSize));
    }
}