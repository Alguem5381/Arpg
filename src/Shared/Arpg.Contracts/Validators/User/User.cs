using Arpg.Contracts.Dto.User;
using Arpg.Primitives.Codes;
using FluentValidation;

namespace Arpg.Contracts.Validators.User;

public class NewDtoValidator : AbstractValidator<NewUserDto>
{
    public NewDtoValidator()
    {
        RuleFor(x => x.Username)
            .Length(2, 50)
            .WithMessage("The username must be at least 2 characters long.")
            .WithErrorCode(DataFormatCodes.InvalidSize)
            .Matches(@"^[a-zA-Z0-9_]+$")
            .WithMessage("The username can only contain letters, numbers, and underscores.")
            .WithErrorCode(UserCodes.InvalidUsernameFormat)
            .NotEmpty()
            .WithMessage("Username is required.")
            .WithErrorCode(DataFormatCodes.Required);

        RuleFor(x => x.Password)
            .Length(8, 50)
            .WithMessage("The password must be at least 8 characters long.")
            .WithErrorCode(DataFormatCodes.InvalidSize)
            .NotEmpty()
            .WithMessage("A password is required.")
            .WithErrorCode(DataFormatCodes.Required);

        RuleFor(x => x.ConfirmPassword)
            .Length(8, 50)
            .WithMessage("The password must be at least 8 characters long.")
            .WithErrorCode(DataFormatCodes.InvalidSize)
            .Equal(x => x.Password)
            .WithMessage("The password must match.")
            .WithErrorCode(UserCodes.PasswordMismatch)
            .NotEmpty()
            .WithMessage("A password is required.")
            .WithErrorCode(DataFormatCodes.Required);

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Invalid email.")
            .WithErrorCode(DataFormatCodes.InvalidEmail)
            .NotEmpty()
            .WithMessage("Email is required.")
            .WithErrorCode(DataFormatCodes.Required);
    }
}

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Username)
            .Length(2, 50)
            .WithMessage("The username must be at least 2 characters long.")
            .WithErrorCode(DataFormatCodes.InvalidSize)
            .Matches(@"^[a-zA-Z0-9_]+$")
            .WithMessage("The username can only contain letters, numbers, and underscores.")
            .WithErrorCode(UserCodes.InvalidUsernameFormat)
            .NotEmpty()
            .WithMessage("Username is required.")
            .WithErrorCode(DataFormatCodes.Required);

        RuleFor(x => x.Password)
            .Length(8, 50)
            .WithMessage("The password must be at least 8 characters long.")
            .WithErrorCode(DataFormatCodes.InvalidSize)
            .NotEmpty()
            .WithMessage("A password is required.")
            .WithErrorCode(DataFormatCodes.Required);
    }
}

public class EditDtoValidator : AbstractValidator<EditUserDto>
{
    public EditDtoValidator()
    {
        RuleFor(x => x.DisplayName)
            .NotEmpty()
            .WithMessage("The display name is required.")
            .WithErrorCode(DataFormatCodes.Required)
            .Length(2, 50)
            .WithMessage("The diplay name must be at least 2 characters long.")
            .WithErrorCode(DataFormatCodes.InvalidSize)
            .Matches(@"^[\p{L}\p{N}_]+$")
            .WithMessage("The username can only contain letters, numbers, and underscores.")
            .WithErrorCode(UserCodes.InvalidDisplayNameFormat);
    }
}

public class DeleteDtoValidator : AbstractValidator<DeleteUserDto>
{
    public DeleteDtoValidator()
    {
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Your password is required.")
            .WithErrorCode(DataFormatCodes.Required)
            .Length(8, 50)
            .WithMessage("The password must be at least 8 characters long.")
            .WithErrorCode(DataFormatCodes.InvalidSize);
    }
}

public class ValidateCodeDtoValidator : AbstractValidator<ValidateCodeDto>
{
    public ValidateCodeDtoValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty()
            .WithMessage("Security key is required.")
            .WithErrorCode(DataFormatCodes.Required);

        RuleFor(x => x.Value)
            .Length(6, 6)
            .WithMessage("The code must have exactly 6 digits.")
            .WithErrorCode(DataFormatCodes.InvalidUniqueSize);
    }
}