using Arpg.Client.Abstractions;
using Arpg.Client.ViewModels.Auth;

namespace Arpg.Client.ViewModels;

public class MainViewModel : ViewModelBase
{
    public INavigationServices NavigationServices { get; }

    public ViewModelBase CurrentPage => NavigationServices.CurrentViewModel;

    public MainViewModel(INavigationServices nav)
    {
        NavigationServices = nav;
        StartApp();
    }

    private void StartApp()
    {
        NavigateToLogin();
    }

    private void NavigateToRegister()
    {
        NavigationServices.JumpTo<RegisterViewModel>(vm =>
        {
            vm.NavigateToLogin = NavigateToLogin;
        });
    }

    private void NavigateToLogin()
    {
        NavigationServices.JumpTo<LoginViewModel>(vm =>
        {
            vm.NavigateToRegister = NavigateToRegister;
        });
    }
}