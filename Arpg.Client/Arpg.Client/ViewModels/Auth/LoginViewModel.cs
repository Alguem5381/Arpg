using System;
using System.Threading.Tasks;
using Arpg.Client.Abstractions;
using Arpg.Client.Core;
using Arpg.Contracts.Dto.User;
using Arpg.Shared.Constants;
using Arpg.Shared.Codes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Arpg.Client.ViewModels.Auth;

public partial class LoginViewModel(IAuthServices authServices) : JumpableViewModel
{
    [ObservableProperty] 
    private string _username = string.Empty;
    [ObservableProperty]
    private string _usernameError = string.Empty;
    [ObservableProperty]
    private bool _usernameErrorState;
    private TransitionToken _usernameErrorToken = new();
    
    [ObservableProperty] 
    private string _password = string.Empty;
    [ObservableProperty] 
    private string _passwordError = string.Empty;
    [ObservableProperty]
    private bool _passwordErrorState;
    private TransitionToken _passwordErrorToken = new();
    

    [ObservableProperty]
    private string _errorMessage = string.Empty;
    [ObservableProperty]
    private bool _errorMessageState;
    private TransitionToken _errorMessageToken = new();

    [ObservableProperty]
    private bool _isLoading;
    private TransitionToken _isLoadingToken = new();

    public Action? NavigateToRegister { get; set; }
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
        
        var response = await authServices.LoginAsync(new (Username, Password));
        
        var usernameErrorIntermediate = string.Empty;
        var passwordErrorIntermediate = string.Empty;
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
                {
                    if (responseError.Metadata.TryGetValue(MetadataKey.PropertyName, out var value))
                        switch (value as string)
                        {
                            case nameof(LoginDto.Username):
                                usernameErrorIntermediate = ErrorTranslator.ToMessage(responseError); break;
                            case nameof(LoginDto.Password):
                                passwordErrorIntermediate = ErrorTranslator.ToMessage(responseError); break;
                        }
                } break;
                case UserCodes.InvalidCredentials:
                {
                    errorMessageIntermediate = ErrorTranslator.ToMessage(responseError);
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

        var apply1 = ApplyChangeAsync(
            usernameErrorIntermediate,
            e => UsernameError = e,
            state => UsernameErrorState = state,
            () => _usernameErrorToken,
            token => _usernameErrorToken = token);
        
        var apply2 = ApplyChangeAsync(
            passwordErrorIntermediate,
            e => PasswordError = e,
            state => PasswordErrorState = state,
            () => _passwordErrorToken,
            token => _passwordErrorToken = token);
        
        var apply3 = ApplyChangeAsync(
            errorMessageIntermediate,
            e => ErrorMessage = e,
            state => ErrorMessageState = state,
            () => _errorMessageToken,
            token => _errorMessageToken = token);
        
        await Task.WhenAll(apply1, apply2, apply3);
    }
    
    [RelayCommand]
    private void GoToRegister()
        => NavigateToRegister?.Invoke();
}