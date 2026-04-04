using System;
using System.Threading.Tasks;
using Arpg.Client.Controls.Common;
using Arpg.Client.Extensions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Arpg.Client.Styles.Controls;

public class AnimatedCodeBox : TextBox
{
    private TransitionToken _stateToken = TransitionToken.Create();
    private TransitionToken _errorStateToken = TransitionToken.Create();

    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<AnimatedCodeBox, bool>(nameof(IsActive));

    public bool IsActive
    {
        get => GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public static readonly StyledProperty<double> InternalMaxHeightProperty =
        AvaloniaProperty.Register<AnimatedCodeBox, double>(nameof(InternalMaxHeight));

    public double InternalMaxHeight
    {
        get => GetValue(InternalMaxHeightProperty);
        set => SetValue(InternalMaxHeightProperty, value);
    }

    public static readonly StyledProperty<double> InternalOpacityProperty =
        AvaloniaProperty.Register<AnimatedCodeBox, double>(nameof(InternalOpacity));

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
        AvaloniaProperty.Register<AnimatedCodeBox, double>(nameof(InternalErrorHeight));

    public double InternalErrorHeight
    {
        get => GetValue(InternalErrorHeightProperty);
        set => SetValue(InternalErrorHeightProperty, value);
    }

    public static readonly StyledProperty<double> InternalErrorOpacityProperty =
        AvaloniaProperty.Register<AnimatedCodeBox, double>(nameof(InternalErrorOpacity));

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
        IsActiveProperty.Changed.AddClassHandler<AnimatedCodeBox>((x, e) => x.OnIsActiveChanged(e));
        ErrorProperty.Changed.AddClassHandler<AnimatedCodeBox>((x, _) => x.OnErrorPropertyChanged());
        HasErrorProperty.Changed.AddClassHandler<AnimatedCodeBox>((x, _) => x.OnErrorPropertyChanged());
        BoundsProperty.Changed.AddClassHandler<AnimatedCodeBox>((x, _) => x.OnBoundsChanged());
    }

    private void OnIsActiveChanged(AvaloniaPropertyChangedEventArgs e)
    {
        _ = UpdateAnimationState(e.GetNewValue<bool>());
    }

    private void OnErrorPropertyChanged()
    {
        _ = UpdateErrorAnimation();
    }

    private void OnBoundsChanged()
    {
        if (HasError) _ = UpdateErrorAnimation();
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        MaxLength = 6;

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

    private async Task UpdateAnimationState(bool isActive)
    {
        var token = TransitionToken.Create();
        _stateToken = token;

        if (isActive)
        {
            await Task.Delay(300);
            if (_stateToken != token) return;

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

            if (_stateToken != token) return;

            await Task.Delay(200);
            InternalMaxHeight = 0;
            Margin = new Thickness(0);
        }
    }

    private async Task UpdateErrorAnimation()
    {
        var token = TransitionToken.Create();
        _errorStateToken = token;

        if (HasError && !string.IsNullOrEmpty(Error))
        {
            if (InternalErrorText != Error && InternalErrorOpacity > 0)
            {
                InternalErrorOpacity = 0;
                await Task.Delay(200);
                if (_errorStateToken != token) return;
            }

            InternalErrorText = Error;

            var availableWidth = Math.Max(10, Bounds.Width - 10);
            var targetHeight = Visual.MeasureTextHeight(Error, availableWidth, 12) + 6;

            InternalErrorHeight = targetHeight;
            await Task.Delay(50);

            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                if (_errorStateToken == token) InternalErrorOpacity = 1;
            });
        }
        else
        {
            InternalErrorOpacity = 0;
            await Task.Delay(200);

            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                if (_errorStateToken != token) return;
                
                InternalErrorHeight = 0;
                InternalErrorText = string.Empty;
            });
        }
    }
}
