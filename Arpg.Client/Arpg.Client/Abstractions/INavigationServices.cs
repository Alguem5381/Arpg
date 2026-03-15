using System;
using Arpg.Client.ViewModels;

namespace Arpg.Client.Abstractions;

public interface INavigationServices
{
    ViewModelBase CurrentViewModel { get; }
    public void NavigateTo<TViewModel>(Action<TViewModel>? configure = null) where TViewModel : ViewModelBase;
    public void JumpTo<TViewModel>(Action<TViewModel>? configure = null) where TViewModel : JumpableViewModel;
    void GoBack();
    bool CanGoBack { get; }
}