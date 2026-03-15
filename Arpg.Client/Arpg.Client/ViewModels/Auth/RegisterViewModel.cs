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

public partial class RegisterViewModel(IAuthServices authServices) : JumpableViewModel
{
    [ObservableProperty] 
    private string _username = string.Empty;
    [ObservableProperty]
    private string _usernameError = string.Empty;
    [ObservableProperty]
    private bool _usernameErrorState;
    private TransitionToken _usernameErrorToken = new();
    
    [ObservableProperty] 
    private string _email = string.Empty;
    [ObservableProperty]
    private string _emailError = string.Empty;
    [ObservableProperty]
    private bool _emailErrorState;
    private TransitionToken _emailErrorToken = new();
    
    [ObservableProperty] 
    private string _password = string.Empty;
    [ObservableProperty] 
    private string _passwordError = string.Empty;
    [ObservableProperty]
    private bool _passwordErrorState;
    private TransitionToken _passwordErrorToken = new();
    
    [ObservableProperty] 
    private string _passwordConfirm = string.Empty;
    [ObservableProperty] 
    private string _passwordConfirmError = string.Empty;
    [ObservableProperty]
    private bool _passwordConfirmErrorState;
    private TransitionToken _passwordConfirmErrorToken = new();
    

    [ObservableProperty]
    private string _errorMessage = string.Empty;
    [ObservableProperty]
    private bool _errorMessageState;
    private TransitionToken _errorMessageToken = new();

    [ObservableProperty]
    private bool _isLoading;
    private TransitionToken _isLoadingToken = new();

    public Action? NavigateToLogin { get; set; }
    public Action? NavigateToMainPage { get; set; }

    [RelayCommand]
    private async Task Login()
    {
        _ = ApplyChangeAsync(
            true,
            value => IsLoading = value,
            value => IsLoading = value,
            () => _isLoadingToken,
            token => _isLoadingToken = token);
        
        var response = await authServices.New(new (Username, Email, Password, PasswordConfirm));
        
        var usernameErrorIntermediate = string.Empty;
        var emailErrorIntermediate = string.Empty;
        var passwordErrorIntermediate = string.Empty;
        var passwordConfirmErrorIntermediate = string.Empty;
        var errorMessageIntermediate = string.Empty;
        
        await ApplyChangeAsync(
            false,
            value => IsLoading = value,
            value => IsLoading = value,
            () => _isLoadingToken,
            token => _isLoadingToken = token);
        
        if (!response.IsFailed)
            NavigateToMainPage?.Invoke();
        
        foreach (var error in response.Errors)
        {
            var responseError = error as ApiError;
            switch (responseError?.Code)
            {
                case DataFormatCodes.Required:
                case DataFormatCodes.InvalidSize:
                case UserCodes.UserConflict:
                {
                    if (responseError.Metadata.TryGetValue(MetadataKey.PropertyName, out var value))
                        switch (value as string)
                        {
                            case nameof(NewDto.Username):
                                usernameErrorIntermediate = ErrorTranslator.ToMessage(responseError); break;
                            case nameof(NewDto.Password):
                                passwordErrorIntermediate = ErrorTranslator.ToMessage(responseError); break;
                            case nameof(NewDto.ConfirmPassword):
                                passwordConfirmErrorIntermediate = ErrorTranslator.ToMessage(responseError); break;
                            case nameof(NewDto.Email):
                                emailErrorIntermediate = ErrorTranslator.ToMessage(responseError); break;
                        }
                } break;
                case UserCodes.PasswordMismatch:
                {
                    passwordErrorIntermediate = ErrorTranslator.ToMessage(responseError);
                } break;
                case DataFormatCodes.InvalidEmailFormat:
                {
                    emailErrorIntermediate = ErrorTranslator.ToMessage(responseError);
                } break;
                case UserCodes.InvalidUsernameFormat:
                {
                    usernameErrorIntermediate = ErrorTranslator.ToMessage(responseError);
                } break;
                default:
                {
                    errorMessageIntermediate = ErrorTranslator.ToMessage(responseError);
                } break;
            }
        }

        var applyUsernameError = ApplyChangeAsync(
            usernameErrorIntermediate,
            e => UsernameError = e,
            state => UsernameErrorState = state,
            () => _usernameErrorToken,
            token => _usernameErrorToken = token);
        
        var applyEmailError = ApplyChangeAsync(
            emailErrorIntermediate,
            e => EmailError = e,
            state => EmailErrorState = state,
            () => _emailErrorToken,
            token => _emailErrorToken = token);
        
        var applyPasswordError = ApplyChangeAsync(
            passwordErrorIntermediate,
            e => PasswordError = e,
            state => PasswordErrorState = state,
            () => _passwordErrorToken,
            token => _passwordErrorToken = token);
        
        var applyPasswordConfirmError = ApplyChangeAsync(
            passwordConfirmErrorIntermediate,
            e => PasswordConfirmError = e,
            state => PasswordConfirmErrorState = state,
            () => _passwordConfirmErrorToken,
            token => _passwordConfirmErrorToken = token);
        
        var applyErrorMessage = ApplyChangeAsync(
            errorMessageIntermediate,
            e => ErrorMessage = e,
            state => ErrorMessageState = state,
            () => _errorMessageToken,
            token => _errorMessageToken = token);
        
        await Task.WhenAll(applyUsernameError, applyEmailError, applyPasswordError, applyPasswordConfirmError, applyErrorMessage);
    }
    
    [RelayCommand]
    private void GoToLogin()
        => NavigateToLogin?.Invoke();
}