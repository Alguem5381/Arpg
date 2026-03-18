using System;
using System.Threading.Tasks;
using Arpg.Client.Abstractions;
using Arpg.Client.Core;
using Arpg.Contracts.Dto.User;
using Arpg.Shared.Codes;
using Arpg.Shared.Constants;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Arpg.Client.ViewModels.Auth;

public enum AuthViewMode { Login, Register, Validate }

public partial class LoginViewModel(IAuthServices authServices) : JumpableViewModel
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsRegisterMode))]
    [NotifyPropertyChangedFor(nameof(IsValidateMode))]
    [NotifyPropertyChangedFor(nameof(SubmitButtonText))]
    [NotifyPropertyChangedFor(nameof(ToggleButtonText))]
    [NotifyPropertyChangedFor(nameof(ModeSubtitle))]
    [NotifyPropertyChangedFor(nameof(ValidateNotificationText))]
    private AuthViewMode _currentMode = AuthViewMode.Login;

    public bool IsRegisterMode => CurrentMode == AuthViewMode.Register;
    public bool IsValidateMode => CurrentMode == AuthViewMode.Validate;
    
    public string SubmitButtonText => CurrentMode switch
    {
        AuthViewMode.Login => "Entrar",
        AuthViewMode.Register => "Registrar",
        AuthViewMode.Validate => "Validar",
        _ => "Continuar"
    };

    public string ToggleButtonText => CurrentMode switch
    {
        AuthViewMode.Login => "Criar conta",
        AuthViewMode.Register => "Voltar",
        AuthViewMode.Validate => "Voltar ao Login",
        _ => "Voltar"
    };

    public string ModeSubtitle => CurrentMode switch
    {
        AuthViewMode.Login => "ENTRAR",
        AuthViewMode.Register => "CRIAR CONTA",
        AuthViewMode.Validate => "VALIDAR CÓDIGO",
        _ => ""
    };

    public string ValidateNotificationText => CurrentMode == AuthViewMode.Validate 
        ? "Um código de validação foi enviado para o seu e-mail." 
        : string.Empty;

    [ObservableProperty] private string _username = string.Empty;
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private string _confirmPassword = string.Empty;
    [ObservableProperty] private string _code = string.Empty;
    [ObservableProperty] private bool _revealPassword = false;

    public Guid ValidationKey { get; private set; }

    [ObservableProperty] private AnimatedState<string> _usernameError = new();
    [ObservableProperty] private AnimatedState<string> _emailError = new();
    [ObservableProperty] private AnimatedState<string> _passwordError = new();
    [ObservableProperty] private AnimatedState<string> _confirmPasswordError = new();
    [ObservableProperty] private AnimatedState<string> _codeError = new();
    [ObservableProperty] private AnimatedState<string> _errorMessage = new();
    [ObservableProperty] private AnimatedState<bool> _isLoading = new();
    [ObservableProperty] private bool _isOperating = false;

    private Guid _operationToken;

    public Action? NavigateToMainPage { get; set; }

    private async void StartOperation()
    {
        IsOperating = true;
        var token = Guid.NewGuid();
        _operationToken = token;

        await Task.Delay(300);

        if (_operationToken == token)
        {
            _ = IsLoading.ApplyAsync(true);
        }
    }

    private void EndOperation()
    {
        _operationToken = Guid.Empty;
        _ = IsLoading.ApplyAsync(false);
        IsOperating = false;
    }

    [RelayCommand]
    private async Task Submit()
    {
        if (IsOperating) return;
        StartOperation();

        if (CurrentMode == AuthViewMode.Validate)
        {
            await SubmitValidationAsync();
            return;
        }

        var response = CurrentMode == AuthViewMode.Login
            ? await authServices.LoginAsync(new(Username, Password))
            : await authServices.NewAsync(new(Username, Email, Password, ConfirmPassword));

        var usernameErrorIntermediate = string.Empty;
        var emailErrorIntermediate = string.Empty;
        var passwordErrorIntermediate = string.Empty;
        var confirmPasswordErrorIntermediate = string.Empty;
        var errorMessageIntermediate = string.Empty;

        EndOperation();

        if (!response.IsFailed)
        {
            ValidationKey = response.Value.Key;
            
            // Clear current errors before transitioning
            _ = UsernameError.ApplyAsync(string.Empty);
            _ = EmailError.ApplyAsync(string.Empty);
            _ = PasswordError.ApplyAsync(string.Empty);
            _ = ConfirmPasswordError.ApplyAsync(string.Empty);
            _ = ErrorMessage.ApplyAsync(string.Empty);
            
            CurrentMode = AuthViewMode.Validate;
            return;
        }

        foreach (var error in response.Errors)
        {
            var responseError = error as ApiError;

            switch (responseError?.Code)
            {
                case UserCodes.PasswordMismatch:
                    passwordErrorIntermediate = ErrorTranslator.ToMessage(responseError);
                    break;
                case DataFormatCodes.InvalidEmail:
                    emailErrorIntermediate = ErrorTranslator.ToMessage(responseError);
                    break;
                case UserCodes.UserConflict:
                case UserCodes.InvalidUsernameFormat:
                    // If backend provided property metadata, use it
                    if (responseError.Metadata.TryGetValue(MetadataKey.PropertyName, out var value) && value is string propName)
                    {
                        if (propName == nameof(LoginDto.Username) || propName == nameof(NewDto.Username))
                            usernameErrorIntermediate = ErrorTranslator.ToMessage(responseError);
                        else if (propName == nameof(LoginDto.Password) || propName == nameof(NewDto.Password))
                            passwordErrorIntermediate = ErrorTranslator.ToMessage(responseError);
                        else if (propName == nameof(NewDto.Email))
                            emailErrorIntermediate = ErrorTranslator.ToMessage(responseError);
                        else if (propName == nameof(NewDto.ConfirmPassword))
                            confirmPasswordErrorIntermediate = ErrorTranslator.ToMessage(responseError);
                    }
                    else
                    {
                        // Fallback: these codes are intrinsically related to username
                        usernameErrorIntermediate = ErrorTranslator.ToMessage(responseError);
                    }
                    break;
                case DataFormatCodes.Required:
                case DataFormatCodes.InvalidSize:
                    {
                        if (responseError.Metadata.TryGetValue(MetadataKey.PropertyName, out var reqValue))
                        {
                            var reqPropName = reqValue as string;
                            if (reqPropName == nameof(LoginDto.Username) || reqPropName == nameof(NewDto.Username))
                                usernameErrorIntermediate = ErrorTranslator.ToMessage(responseError);
                            else if (reqPropName == nameof(LoginDto.Password) || reqPropName == nameof(NewDto.Password))
                                passwordErrorIntermediate = ErrorTranslator.ToMessage(responseError);
                            else if (reqPropName == nameof(NewDto.Email))
                                emailErrorIntermediate = ErrorTranslator.ToMessage(responseError);
                            else if (reqPropName == nameof(NewDto.ConfirmPassword))
                                confirmPasswordErrorIntermediate = ErrorTranslator.ToMessage(responseError);
                        }
                    }
                    break;
                case UserCodes.InvalidCredentials:
                default:
                    errorMessageIntermediate = ErrorTranslator.ToMessage(responseError);
                    break;
            }
        }

        var apply1 = UsernameError.ApplyAsync(usernameErrorIntermediate);
        var apply2 = EmailError.ApplyAsync(emailErrorIntermediate);
        var apply3 = PasswordError.ApplyAsync(passwordErrorIntermediate);
        var apply4 = ConfirmPasswordError.ApplyAsync(confirmPasswordErrorIntermediate);
        var apply5 = ErrorMessage.ApplyAsync(errorMessageIntermediate);

        await Task.WhenAll(apply1, apply2, apply3, apply4, apply5);
    }

    private async Task SubmitValidationAsync()
    {
        var codeErrorIntermediate = string.Empty;
        var errorMessageIntermediate = string.Empty;

        var response = await authServices.ValidateAsync(new(ValidationKey, Code));

        EndOperation();

        if (!response.IsFailed)
        {
            NavigateToMainPage?.Invoke();
            return;
        }

        foreach (var error in response.Errors)
        {
            var responseError = error as ApiError;

            switch (responseError?.Code)
            {
                case CodeCodes.CodeNotFound:
                case CodeCodes.InvalidCode:
                case CodeCodes.CodeExpired:
                case DataFormatCodes.Required:
                case DataFormatCodes.InvalidSize:
                    // Se houver metadata com o nome da propriedade e for Code, atribui ao campo do codigo. 
                    // Como a tela de Validate só tem o Code, podemos até assumir que falhas de formato no Validate são do Code.
                    codeErrorIntermediate = ErrorTranslator.ToMessage(responseError);
                    break;
                case GeneralCodes.NoConnection:
                case GeneralCodes.Timeout:
                case GeneralCodes.Domain:
                default:
                    errorMessageIntermediate = ErrorTranslator.ToMessage(responseError);
                    break;
            }
        }

        var apply1 = CodeError.ApplyAsync(codeErrorIntermediate);
        var apply2 = ErrorMessage.ApplyAsync(errorMessageIntermediate);
        await Task.WhenAll(apply1, apply2);
    }

    [RelayCommand]
    private async Task ToggleMode()
    {
        if (CurrentMode == AuthViewMode.Validate)
            CurrentMode = AuthViewMode.Login;
        else
            CurrentMode = CurrentMode == AuthViewMode.Login ? AuthViewMode.Register : AuthViewMode.Login;

        // Clear everything when toggling
        var a1 = UsernameError.ApplyAsync(string.Empty);
        var a2 = EmailError.ApplyAsync(string.Empty);
        var a3 = PasswordError.ApplyAsync(string.Empty);
        var a4 = ConfirmPasswordError.ApplyAsync(string.Empty);
        var a5 = CodeError.ApplyAsync(string.Empty);
        var a6 = ErrorMessage.ApplyAsync(string.Empty);

        await Task.WhenAll(a1, a2, a3, a4, a5, a6);
    }

}