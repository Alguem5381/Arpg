using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Arpg.Client.ViewModels;

public abstract partial class ViewModelBase : ObservableObject
{
    public partial class AnimatedState<T> : ObservableObject
    {
        [ObservableProperty] private T? _value;
        [ObservableProperty] private bool _show;
        public TransitionToken Token = new();

        public Task ApplyAsync(T? value, int time = 0)
        {
            Value = value;

            var shouldShow = value != null;
            if (value is string s && string.IsNullOrEmpty(s) || value is bool and false)
                shouldShow = false;

            Show = shouldShow;
            return Task.CompletedTask;
        }
    }

    public class TransitionToken
    {
        public Guid Id = Guid.NewGuid();
    }

    protected Task ApplyChangeAsync<T>(
        T value, 
        Action<T> updateData, 
        Action<bool> updateState,
        Func<TransitionToken> getToken,
        Action<TransitionToken> setToken,
        int time = 0)
    {
        updateData(value);

        var shouldShow = value != null;
        if (value is string s && string.IsNullOrEmpty(s) || value is bool and false)
            shouldShow = false;
        
        updateState(shouldShow);

        return Task.CompletedTask;
    }
}

