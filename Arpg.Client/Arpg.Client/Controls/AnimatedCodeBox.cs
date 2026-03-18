using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Arpg.Client.Controls;

public class AnimatedCodeBox : TextBox
{
    private TransitionToken _stateToken = new();
    private TransitionToken _errorStateToken = new();

    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<AnimatedCodeBox, bool>(nameof(IsActive), false);

    public bool IsActive
    {
        get => GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public static readonly StyledProperty<double> InternalMaxHeightProperty =
        AvaloniaProperty.Register<AnimatedCodeBox, double>(nameof(InternalMaxHeight), 0.0);

    public double InternalMaxHeight
    {
        get => GetValue(InternalMaxHeightProperty);
        set => SetValue(InternalMaxHeightProperty, value);
    }

    public static readonly StyledProperty<double> InternalOpacityProperty =
        AvaloniaProperty.Register<AnimatedCodeBox, double>(nameof(InternalOpacity), 0.0);

    public double InternalOpacity
    {
        get => GetValue(InternalOpacityProperty);
        set => SetValue(InternalOpacityProperty, value);
    }

    public static readonly StyledProperty<Thickness> TargetMarginProperty =
        AvaloniaProperty.Register<AnimatedCodeBox, Thickness>(nameof(TargetMargin));

    public Thickness TargetMargin
    {
        get => GetValue(TargetMarginProperty);
        set => SetValue(TargetMarginProperty, value);
    }

    public static readonly StyledProperty<string?> ErrorProperty =
        AvaloniaProperty.Register<AnimatedCodeBox, string?>(nameof(Error));

    public string? Error
    {
        get => GetValue(ErrorProperty);
        set => SetValue(ErrorProperty, value);
    }

    public static readonly StyledProperty<bool> HasErrorProperty =
        AvaloniaProperty.Register<AnimatedCodeBox, bool>(nameof(HasError));

    public bool HasError
    {
        get => GetValue(HasErrorProperty);
        set => SetValue(HasErrorProperty, value);
    }

    public static readonly StyledProperty<double> InternalErrorHeightProperty =
        AvaloniaProperty.Register<AnimatedCodeBox, double>(nameof(InternalErrorHeight), 0.0);

    public double InternalErrorHeight
    {
        get => GetValue(InternalErrorHeightProperty);
        set => SetValue(InternalErrorHeightProperty, value);
    }

    public static readonly StyledProperty<double> InternalErrorOpacityProperty =
        AvaloniaProperty.Register<AnimatedCodeBox, double>(nameof(InternalErrorOpacity), 0.0);

    public double InternalErrorOpacity
    {
        get => GetValue(InternalErrorOpacityProperty);
        set => SetValue(InternalErrorOpacityProperty, value);
    }

    public static readonly StyledProperty<string?> InternalErrorTextProperty =
        AvaloniaProperty.Register<AnimatedCodeBox, string?>(nameof(InternalErrorText));

    public string? InternalErrorText
    {
        get => GetValue(InternalErrorTextProperty);
        set => SetValue(InternalErrorTextProperty, value);
    }

    public static readonly StyledProperty<IBrush> ErrorForegroundProperty =
        AvaloniaProperty.Register<AnimatedCodeBox, IBrush>(nameof(ErrorForeground));

    public IBrush ErrorForeground
    {
        get => GetValue(ErrorForegroundProperty);
        set => SetValue(ErrorForegroundProperty, value);
    }

    static AnimatedCodeBox()
    {
        IsActiveProperty.Changed.AddClassHandler<AnimatedCodeBox>((x, e) => x.OnActiveChanged(e));
    }

    private void OnActiveChanged(AvaloniaPropertyChangedEventArgs e)
    {
        UpdateAnimationState(e.GetNewValue<bool>());
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        MaxLength = 6;

        // Sync initial state without transitions
        if (!IsActive)
        {
            InternalMaxHeight = 0;
            InternalOpacity = 0;
            Margin = new Thickness(0);
            IsEnabled = false;
            IsTabStop = false;
            Focusable = false;
        }
        else
        {
            InternalMaxHeight = 150;
            InternalOpacity = 1;
            Margin = TargetMargin;
            IsEnabled = true;
            IsTabStop = true;
            Focusable = true;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == HasErrorProperty || change.Property == ErrorProperty)
        {
            UpdateErrorAnimation();
        }

        if (change.Property == BoundsProperty)
        {
            if (HasError) UpdateErrorAnimation();
        }
    }

    private async void UpdateAnimationState(bool isActive)
    {
        var token = new TransitionToken();
        _stateToken = token;

        if (isActive)
        {
            // Atraso inicial para esperar elementos sendo escondidos terminarem seu fade out
            await Task.Delay(300);
            if (token != _stateToken) return;

            IsEnabled = true;
            IsTabStop = true;
            Focusable = true;

            Margin = TargetMargin;
            InternalMaxHeight = 150;
            await Task.Delay(300);
            if (token != _stateToken) return;
            InternalOpacity = 1;
        }
        else
        {
            InternalOpacity = 0;
            IsEnabled = false;
            IsTabStop = false;
            Focusable = false;

            if (token != _stateToken) return;

            await Task.Delay(200);
            InternalMaxHeight = 0;
            Margin = new Thickness(0);
        }
    }

    private async void UpdateErrorAnimation()
    {
        var token = new TransitionToken();
        _errorStateToken = token;

        if (HasError && !string.IsNullOrEmpty(Error))
        {
            if (InternalErrorText != Error && InternalErrorOpacity > 0)
            {
                InternalErrorOpacity = 0;
                await Task.Delay(200);
                if (token != _errorStateToken) return;
            }

            InternalErrorText = Error;

            var textBlock = new TextBlock
            {
                Text = Error,
                FontSize = 12,
                TextWrapping = TextWrapping.Wrap
            };

            var availableWidth = Math.Max(10, Bounds.Width - 10);
            textBlock.Measure(new Size(availableWidth, double.PositiveInfinity));

            var targetHeight = textBlock.DesiredSize.Height + 6;

            InternalErrorHeight = targetHeight;
            await Task.Delay(50);

            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                if (token == _errorStateToken) InternalErrorOpacity = 1;
            });
        }
        else
        {
            InternalErrorOpacity = 0;
            await Task.Delay(200);

            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                if (token == _errorStateToken)
                {
                    InternalErrorHeight = 0;
                    InternalErrorText = string.Empty;
                }
            });
        }
    }

    private class TransitionToken
    {
        public Guid Id = Guid.NewGuid();
    }
}
