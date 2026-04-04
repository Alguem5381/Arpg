using System;
using System.Collections.Generic;
using Arpg.Client.Abstractions;
using Arpg.Client.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using Arpg.Client.ViewModels.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Arpg.Client.Services;

public partial class NavigationService(IServiceProvider serviceProvider) : ObservableObject, INavigationServices
{
    private readonly Stack<ViewModelBase> _history = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanGoBack))]
    private ViewModelBase _currentViewModel = null!;

    public bool CanGoBack => _history.Count > 0;

    public void NavigateTo<TViewModel>(Action<TViewModel>? configure = null)
    where TViewModel : ViewModelBase
    {
        if (CurrentViewModel != null && CurrentViewModel is not JumpableViewModel) 
            _history.Push(CurrentViewModel);

        var vm = serviceProvider.GetRequiredService<TViewModel>();

        configure?.Invoke(vm);

        CurrentViewModel = vm;
    }
    
    public void JumpTo<TViewModel>(Action<TViewModel>? configure = null)
        where TViewModel : JumpableViewModel
    {
        if (CurrentViewModel != null && CurrentViewModel is not JumpableViewModel jumpableViewModel)
            _history.Push(CurrentViewModel);
        
        var vm = serviceProvider.GetRequiredService<TViewModel>();

        configure?.Invoke(vm);

        CurrentViewModel = vm;
    }

    public void GoBack()
    {
        if (_history.Count > 0)
        {
            var previousViewModel = _history.Pop();
            CurrentViewModel = previousViewModel;
        }
    }
}