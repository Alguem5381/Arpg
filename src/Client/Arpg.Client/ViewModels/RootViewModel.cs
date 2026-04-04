using Arpg.Client.Abstractions;
using Arpg.Client.ViewModels.Common;
using Arpg.Client.ViewModels.Auth;

namespace Arpg.Client.ViewModels;

public class RootViewModel : ViewModelBase
{
    public INavigationServices NavigationServices { get; }

    public ViewModelBase CurrentPage => NavigationServices.CurrentViewModel;

    public RootViewModel(INavigationServices nav)
    {
        NavigationServices = nav;
        StartApp();
    }

    private void StartApp()
    {
        NavigateToLogin();
    }

    private void NavigateToMainPage()
    {
        // This should probably navigate to a dashboard or main menu
        // For now, let's just log or keep it as a placeholder if needed
    }

    private void NavigateToLogin()
    {
        NavigationServices.JumpTo<AuthViewModel>(vm =>
        {
            vm.AuthenticationCompleted += NavigateToMainPage;
        });
    }
}