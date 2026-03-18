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

    private void NavigateToMainPage()
    {
        // This should probably navigate to a dashboard or main menu
        // For now, let's just log or keep it as a placeholder if needed
    }

    private void NavigateToLogin()
    {
        NavigationServices.JumpTo<LoginViewModel>(vm =>
        {
            vm.NavigateToMainPage = NavigateToMainPage;
        });
    }
}