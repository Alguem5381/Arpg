using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Arpg.Client.Core;
using Arpg.Primitives.Constants;
using FluentValidation.Results;
using CommunityToolkit.Mvvm.ComponentModel;
using Arpg.Client.Extensions;

namespace Arpg.Client.ViewModels.Common;

public partial class FormField<T>
(
    T initialValue,
    string propertyName,
    Func<Task<ValidationResult?>>? validationAction = null,
    string[]? codes = null,
    int debounceMs = 300
) : ObservableObject
{
    private T _value = initialValue;
    private string _error = string.Empty;

    private readonly Func<Task<ValidationResult?>>? _validationAction = validationAction;
    private readonly string[] _targetCodes = codes ?? [];
    private readonly string _propertyName = propertyName;
    private readonly int _debounceMs = debounceMs;
    private CancellationTokenSource? _debounceTcs;

    public T Value
    {
        get => _value;
        set
        {
            if (SetProperty(ref _value, value))
            {
                if (_validationAction != null)
                {
                    TriggerDebounceValidation();
                }
            }
        }
    }

    public string Error
    {
        get => _error;
        set => SetProperty(ref _error, value);
    }

    public void SetError(string message) => Error = message;
    public void ClearError() => Error = string.Empty;

    public bool TryResolve(ApiError error)
    {
        if (error.Metadata.TryGetValue(MetadataKey.PropertyName, out var propObj) && propObj is string propName)
        {
            if (propName == _propertyName)
            {
                Error = ErrorTranslator.ToMessage(error);
                return true;
            }
        }

        if (Enumerable.Contains(_targetCodes, error.Code))
        {
            Error = ErrorTranslator.ToMessage(error);
            return true;
        }

        return false;
    }

    private async void TriggerDebounceValidation()
    {
        _debounceTcs?.Cancel();
        _debounceTcs = new CancellationTokenSource();
        var token = _debounceTcs.Token;

        try
        {
            await Task.Delay(_debounceMs, token);

            if (!token.IsCancellationRequested)
            {
                var result = await _validationAction!.Invoke();
                if (result == null)
                    return;

                if (result.IsValid)
                {
                    ClearError();
                    return;
                }

                var lastError = result.ToApiError().Errors.LastOrDefault();

                if (lastError is ApiError error)
                    SetError(ErrorTranslator.ToMessage(error));
                else
                    ClearError();
            }
        }
        catch (TaskCanceledException)
        {
            // O cancelamento é esperado numa digitação contínua
        }
    }
}
