using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Arpg.Client.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
    protected class TransitionToken
    {
        public Guid Id = Guid.NewGuid();
    }

    protected async Task ApplyChangeAsync<T>(
        T value, 
        Action<T> updateData, 
        Action<bool> updateState,
        Func<TransitionToken> getToken,
        Action<TransitionToken> setToken,
        int time = 500)
    {
        TransitionToken myToken = new();
        setToken(myToken);
        
        updateState(false);

        await Task.Delay(time);

        if (myToken != getToken())
        {
            return;
        }
        
        updateData(value);

        var shouldShow = value != null;
        if (value is string s && string.IsNullOrEmpty(s) || value is bool and false)
            shouldShow = false;
        
        if (shouldShow)
            updateState(true);
    }
}
