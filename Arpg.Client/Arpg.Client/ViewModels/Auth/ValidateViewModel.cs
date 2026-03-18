using System;
using System.Threading.Tasks;
using Arpg.Client.Abstractions;
using Arpg.Client.Core;
using Arpg.Contracts.Dto.User;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Arpg.Client.ViewModels.Auth;

public partial class ValidateViewModel(IAuthServices authServices) : JumpableViewModel
{
    private Guid _key;

    [ObservableProperty] private string _code = string.Empty;
    [ObservableProperty] private AnimatedState<string> _codeError = new();
    [ObservableProperty] private AnimatedState<string> _errorMessage = new();
    [ObservableProperty] private AnimatedState<bool> _isLoading = new();

    public Action? NavigateToMainPage { get; set; }
    public Action? NavigateBack { get; set; }

    public void Initialize(Guid key) => _key = key;

    [RelayCommand]
    private async Task Validate()
    {
        if (string.IsNullOrWhiteSpace(Code))
        {
            await CodeError.ApplyAsync("O código é obrigatório.");
            return;
        }

        _ = IsLoading.ApplyAsync(true);

        var response = await authServices.ValidateAsync(new ValidateCodeDto(_key, Code));

        await IsLoading.ApplyAsync(false);

        if (!response.IsFailed)
        {
            NavigateToMainPage?.Invoke();
            return;
        }

        var errorMessageIntermediate = string.Empty;

        foreach (var error in response.Errors)
        {
            var responseError = error as ApiError;
            errorMessageIntermediate = ErrorTranslator.ToMessage(responseError);
        }

        await ErrorMessage.ApplyAsync(errorMessageIntermediate);
    }

    [RelayCommand]
    private void GoBack() => NavigateBack?.Invoke();
}
