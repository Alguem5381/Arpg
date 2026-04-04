using System;
using System.Threading.Tasks;
using Arpg.Client.Controls.Common;
using Arpg.Client.Extensions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;

namespace Arpg.Client.Styles.Controls;

public class AnimatedTextBox : TextBox
{
    private TransitionToken _stateToken = TransitionToken.Create();
    private TransitionToken _errorStateToken = TransitionToken.Create();

    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<AnimatedTextBox, bool>(nameof(IsActive), true);

    public bool IsActive
    {
        get => GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public static readonly StyledProperty<double> InternalMaxHeightProperty =
        AvaloniaProperty.Register<AnimatedTextBox, double>(nameof(InternalMaxHeight), 150);

    public double InternalMaxHeight
    {
        get => GetValue(InternalMaxHeightProperty);
        set => SetValue(InternalMaxHeightProperty, value);
    }

    public static readonly StyledProperty<double> InternalOpacityProperty =
        AvaloniaProperty.Register<AnimatedTextBox, double>(nameof(InternalOpacity), 1.0);

    public double InternalOpacity
    {
        get => GetValue(InternalOpacityProperty);
        set => SetValue(InternalOpacityProperty, value);
    }

    public static readonly StyledProperty<string?> ErrorProperty =
        AvaloniaProperty.Register<AnimatedTextBox, string?>(nameof(Error));

    public string? Error
    {
        get => GetValue(ErrorProperty);
        set => SetValue(ErrorProperty, value);
    }

    public static readonly StyledProperty<bool> HasErrorProperty =
        AvaloniaProperty.Register<AnimatedTextBox, bool>(nameof(HasError));

    public bool HasError
    {
        get => GetValue(HasErrorProperty);
        set => SetValue(HasErrorProperty, value);
    }

    public static readonly StyledProperty<double> InternalErrorHeightProperty =
        AvaloniaProperty.Register<AnimatedTextBox, double>(nameof(InternalErrorHeight));

    public double InternalErrorHeight
    {
        get => GetValue(InternalErrorHeightProperty);
        set => SetValue(InternalErrorHeightProperty, value);
    }

    public static readonly StyledProperty<double> InternalErrorOpacityProperty =
        AvaloniaProperty.Register<AnimatedTextBox, double>(nameof(InternalErrorOpacity));

    public double InternalErrorOpacity
    {
        get => GetValue(InternalErrorOpacityProperty);
        set => SetValue(InternalErrorOpacityProperty, value);
    }

    public static readonly StyledProperty<string?> InternalErrorTextProperty =
        AvaloniaProperty.Register<AnimatedTextBox, string?>(nameof(InternalErrorText));

    public string? InternalErrorText
    {
        get => GetValue(InternalErrorTextProperty);
        set => SetValue(InternalErrorTextProperty, value);
    }

    public static readonly StyledProperty<Brush> ErrorForegroundProperty =
        AvaloniaProperty.Register<AnimatedTextBox, Brush>(nameof(ErrorForeground));

    public Brush ErrorForeground
    {
        get => GetValue(ErrorForegroundProperty);
        set => SetValue(ErrorForegroundProperty, value);
    }

    public new static readonly StyledProperty<Brush> WatermarkForegroundProperty =
        AvaloniaProperty.Register<AnimatedTextBox, Brush>(nameof(WatermarkForeground));

    public new Brush WatermarkForeground
    {
        get => GetValue(WatermarkForegroundProperty);
        set => SetValue(WatermarkForegroundProperty, value);
    }

    public static readonly StyledProperty<Brush> OutsideBorderBrushProperty =
        AvaloniaProperty.Register<AnimatedTextBox, Brush>(nameof(OutsideBorderBrush));

    public Brush OutsideBorderBrush
    {
        get => GetValue(OutsideBorderBrushProperty);
        set => SetValue(OutsideBorderBrushProperty, value);
    }

    public static readonly StyledProperty<CornerRadius> OutsideCornerRadiusProperty =
        AvaloniaProperty.Register<AnimatedTextBox, CornerRadius>(nameof(OutsideCornerRadius));

    public CornerRadius OutsideCornerRadius
    {
        get => GetValue(OutsideCornerRadiusProperty);
        set => SetValue(OutsideCornerRadiusProperty, value);
    }

    public static readonly StyledProperty<Thickness> OutsideBorderThicknessProperty =
        AvaloniaProperty.Register<AnimatedTextBox, Thickness>(nameof(OutsideBorderThickness));

    public Thickness OutsideBorderThickness
    {
        get => GetValue(OutsideBorderThicknessProperty);
        set => SetValue(OutsideBorderThicknessProperty, value);
    }

    public static readonly StyledProperty<Thickness> OutsidePaddingProperty =
        AvaloniaProperty.Register<AnimatedTextBox, Thickness>(nameof(OutsidePadding));

    public Thickness OutsidePadding
    {
        get => GetValue(OutsidePaddingProperty);
        set => SetValue(OutsidePaddingProperty, value);
    }

    public static readonly StyledProperty<Brush> OutsideBackgroundProperty =
        AvaloniaProperty.Register<AnimatedTextBox, Brush>(nameof(OutsideBackground));

    public Brush OutsideBackground
    {
        get => GetValue(OutsideBackgroundProperty);
        set => SetValue(OutsideBackgroundProperty, value);
    }

    public static readonly StyledProperty<Brush?> BackgroundOnHoverProperty =
        AvaloniaProperty.Register<AnimatedTextBox, Brush?>(nameof(BackgroundOnHover));

    public Brush? BackgroundOnHover
    {
        get => GetValue(BackgroundOnHoverProperty);
        set => SetValue(BackgroundOnHoverProperty, value);
    }

    public new static readonly StyledProperty<bool> RevealPasswordProperty =
        AvaloniaProperty.Register<AnimatedTextBox, bool>(nameof(RevealPassword));

    public new bool RevealPassword
    {
        get => GetValue(RevealPasswordProperty);
        set => SetValue(RevealPasswordProperty, value);
    }

    static AnimatedTextBox()
    {
        IsActiveProperty.Changed.AddClassHandler<AnimatedTextBox>((x, e) => x.OnIsActiveChanged(e));
        ErrorProperty.Changed.AddClassHandler<AnimatedTextBox>((x, _) => x.OnErrorPropertyChanged());
        HasErrorProperty.Changed.AddClassHandler<AnimatedTextBox>((x, _) => x.OnErrorPropertyChanged());
        BoundsProperty.Changed.AddClassHandler<AnimatedTextBox>((x, _) => x.OnBoundsChanged());
        RevealPasswordProperty.Changed.AddClassHandler<AnimatedTextBox>((x, e) => x.OnRevealPasswordChanged(e));
        PasswordCharProperty.Changed.AddClassHandler<AnimatedTextBox>((x, e) => x.OnPasswordCharChanged(e));
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

    private void OnRevealPasswordChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var value = e.GetNewValue<bool>();
        if (value)
        {
            _originalPasswordChar = PasswordChar;
            PasswordChar = '\0';
        }
        else
        {
            PasswordChar = _originalPasswordChar != '\0' ? _originalPasswordChar : '*';
        }
    }

    private void OnPasswordCharChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (!RevealPassword)
        {
            _originalPasswordChar = e.GetNewValue<char>();
        }
    }

    private char _originalPasswordChar = '\0';

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (!IsActive)
        {
            InternalMaxHeight = 0;
            InternalOpacity = 0;
        }
        else
        {
            InternalMaxHeight = 150;
            InternalOpacity = 1;
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

            InternalMaxHeight = 150;
            await Task.Delay(300);
            InternalOpacity = 1;
        }
        else
        {
            InternalOpacity = 0;

            IsEnabled = false;
            IsTabStop = false;
            Focusable = false;

            if (_stateToken != token) return;

            InternalMaxHeight = 0;
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
                await Task.Delay(300);
                if (_errorStateToken != token) return;
            }

            InternalErrorText = Error;

            var availableWidth = Math.Max(10, Bounds.Width - 5);
            var targetHeight = Visual.MeasureTextHeight(Error, availableWidth, 12) + 6;

            InternalErrorHeight = targetHeight;
            await Task.Delay(300);

            Dispatcher.UIThread.Post(() =>
            {
                if (_errorStateToken == token) InternalErrorOpacity = 1;
            });
        }
        else
        {
            InternalErrorOpacity = 0;
            await Task.Delay(200);

            Dispatcher.UIThread.Post(() =>
            {
                if (_errorStateToken != token) return;

                InternalErrorHeight = 0;
                InternalErrorText = string.Empty;
            });
        }
    }
}