using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Arpg.Client.ViewModels.Common;

/// <summary>
/// Rastreia o ciclo de vida de uma operação assíncrona.
/// <para>
/// <see cref="IsActive"/> fica true imediatamente ao iniciar.
/// <see cref="IsLoading"/> fica true após um atraso, indicando que a operação está demorando.
/// </para>
/// </summary>
public partial class OperationTracker : ObservableObject
{
    private Guid _token;

    [ObservableProperty] private bool _isActive;
    [ObservableProperty] private bool _isLoading;

    public async void Start()
    {
        IsActive = true;
        var token = Guid.NewGuid();
        _token = token;

        await Task.Delay(300);

        if (_token == token)
            IsLoading = true;
    }

    public void End()
    {
        _token = Guid.Empty;
        IsLoading = false;
        IsActive = false;
    }
}
