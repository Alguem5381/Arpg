using Arpg.Client.Abstractions;
using Arpg.Client.ViewModels.Common;
using Arpg.Client.ViewModels.Auth;

namespace Arpg.Client.ViewModels;

public class RootViewModel : ViewModelBase
{
    public INavigationServices NavigationServices { get; }

    public ViewModelBase CurrentPage => NavigationServices.CurrentViewModel;

    private readonly IUserSession _session;

    public RootViewModel(INavigationServices nav, IUserSession session)
    {
        NavigationServices = nav;
        _session = session;
        StartApp();
    }

    private void StartApp()
    {
        if (_session.IsAuthenticated)
        {
            NavigateToMainPage();
        }
        else
        {
            NavigateToLogin();
        }
    }

    private void NavigateToMainPage()
    {
        NavigationServices.NavigateTo<MainViewModel>();
    }

    private void NavigateToLogin()
    {
        NavigationServices.JumpTo<AuthViewModel>(viewModel =>
        {
            viewModel.AuthenticationCompleted += NavigateToMainPage;
        });
    }
}