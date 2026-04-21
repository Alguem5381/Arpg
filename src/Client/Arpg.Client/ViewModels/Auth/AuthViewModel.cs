using System;
using System.Linq;
using System.Threading.Tasks;
using Arpg.Client.Abstractions;
using Arpg.Client.Core;
using Arpg.Contracts.Dto.User;
using Arpg.Primitives.Codes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentResults;
using FluentValidation;
using Arpg.Client.ViewModels.Common;

namespace Arpg.Client.ViewModels.Auth;

public enum AuthViewMode { Login, Register, Validate }

public partial class AuthViewModel : JumpableViewModel
{
    public Guid ValidationKey { get; private set; }
    public OperationTracker Operation { get; } = new();
    public event Func<Task>? AuthenticationCompleting;
    public event Action? AuthenticationCompleted;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsRegisterMode))]
    [NotifyPropertyChangedFor(nameof(IsValidateMode))]
    private AuthViewMode _currentMode = AuthViewMode.Login;
    public bool IsRegisterMode => CurrentMode == AuthViewMode.Register;
    public bool IsValidateMode => CurrentMode == AuthViewMode.Validate;

    public FormField<string> Username { get; }
    public FormField<string> Email { get; }
    public FormField<string> Password { get; }
    public FormField<string> ConfirmPassword { get; }
    public FormField<string> Code { get; }

    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _revealPassword = false;

    private readonly IAuthServices _authServices;
    private readonly FormField<string>[] _fields;
    private readonly IValidator<LoginDto> _loginValidator;
    private readonly IValidator<NewUserDto> _newValidator;
    private readonly IValidator<ValidateCodeDto> _codeValidator;


    public AuthViewModel
    (
        IAuthServices authServices,
        IValidator<LoginDto> loginValidator,
        IValidator<NewUserDto> newValidator,
        IValidator<ValidateCodeDto> codeValidator
    )
    {
        _authServices = authServices;
        _loginValidator = loginValidator;
        _newValidator = newValidator;
        _codeValidator = codeValidator;

        Username = new(
                "",
                nameof(LoginDto.Username),
                () => _loginValidator.ValidateAsync(
                    new(Username!.Value, Password!.Value),
                    o => o.IncludeProperties(nameof(LoginDto.Username))
                )
            );

        Email = new(
            "",
            nameof(NewUserDto.Email),
            () => _newValidator.ValidateAsync(
                new(Username!.Value, Email!.Value, Password!.Value, ConfirmPassword!.Value),
                o => o.IncludeProperties(nameof(NewUserDto.Email))
            ),
            [DataFormatCodes.InvalidEmail]
        );

        Password = new(
            "",
            nameof(LoginDto.Password),
            () => _loginValidator.ValidateAsync(
                new(Username!.Value, Password!.Value),
                o => o.IncludeProperties(nameof(LoginDto.Password))
            ),
            [UserCodes.PasswordMismatch]
        );

        ConfirmPassword = new(
            "",
            nameof(NewUserDto.ConfirmPassword),
            () => _newValidator.ValidateAsync(
                new(Username!.Value, Email!.Value, Password!.Value, ConfirmPassword!.Value),
                o => o.IncludeProperties(nameof(NewUserDto.ConfirmPassword))
            )
        );

        Code = new(
            "",
            nameof(ValidateCodeDto.Value),
            () => _codeValidator.ValidateAsync(
                new(ValidationKey, Code!.Value),
                o => o.IncludeProperties(nameof(ValidateCodeDto.Value))
            ),
            [CodeCodes.CodeNotFound, CodeCodes.InvalidCode, CodeCodes.CodeExpired, DataFormatCodes.Required, DataFormatCodes.InvalidSize]
        );

        _fields = [Username, Email, Password, ConfirmPassword, Code];
    }

    [RelayCommand]
    private void ToggleMode()
    {
        if (CurrentMode == AuthViewMode.Login)
            CurrentMode = AuthViewMode.Register;
        else
            CurrentMode = AuthViewMode.Login;

        ClearErrors();
    }

    [RelayCommand]
    private async Task Submit()
    {
        switch (CurrentMode)
        {
            case AuthViewMode.Validate:
                await SubmitValidationAsync();
                break;
            case AuthViewMode.Login:
                await SubmitLoginAsync();
                break;
            case AuthViewMode.Register:
                await SubmitRegisterAsync();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task SubmitLoginAsync()
    {
        if (Operation.IsActive) return;
        Operation.Start();

        var response = await _authServices.LoginAsync(new(Username.Value, Password.Value));

        Operation.End();

        HandleResponseAsync(response);
    }

    private async Task SubmitRegisterAsync()
    {
        if (Operation.IsActive) return;
        Operation.Start();

        var response = await _authServices.NewAsync(new(Username.Value, Email.Value, Password.Value, ConfirmPassword.Value));

        Operation.End();

        HandleResponseAsync(response);
    }

    private void HandleResponseAsync(Result<CodeDto> response)
    {
        if (!response.IsFailed)
        {
            ValidationKey = response.Value.Key;
            ClearErrors();
            CurrentMode = AuthViewMode.Validate;
            return;
        }

        ClearErrors();

        foreach (var error in response.Errors)
        {
            if (error is not ApiError responseError) continue;

            bool resolved = false;

            foreach (var field in _fields)
                if (field.TryResolve(responseError))
                {
                    resolved = true;
                    break;
                }

            if (!resolved)
                ErrorMessage = ErrorTranslator.ToMessage(responseError);
        }
    }

    private async Task SubmitValidationAsync()
    {
        if (Operation.IsActive) return;
        Operation.Start();

        var response = await _authServices.ValidateAsync(new(ValidationKey, Code.Value));

        Operation.End();

        if (!response.IsFailed)
        {
            await OnAuthenticationCompletedAsync();
            return;
        }

        ClearErrors();

        foreach (var error in response.Errors)
        {
            if (error is ApiError responseError &&
                !Code.TryResolve(responseError))
                ErrorMessage = ErrorTranslator.ToMessage(responseError);
        }
    }

    private void ClearErrors()
    {
        foreach (var field in _fields)
            field.ClearError();

        ErrorMessage = string.Empty;
    }

    private async Task OnAuthenticationCompletedAsync()
    {
        if (AuthenticationCompleting != null)
            foreach (var handler in AuthenticationCompleting.GetInvocationList().Cast<Func<Task>>())
                await handler();

        AuthenticationCompleted?.Invoke();
    }
}