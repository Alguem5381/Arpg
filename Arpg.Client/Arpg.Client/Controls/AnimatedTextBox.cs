using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;
using Avalonia.Media;

namespace Arpg.Client.Controls;

public class AnimatedTextBox : TextBox
{
    private TransitionToken _stateToken = new();
    private TransitionToken _errorStateToken = new();

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
        AvaloniaProperty.Register<AnimatedTextBox, double>(nameof(InternalErrorHeight), 0.0);

    public double InternalErrorHeight
    {
        get => GetValue(InternalErrorHeightProperty);
        set => SetValue(InternalErrorHeightProperty, value);
    }

    public static readonly StyledProperty<double> InternalErrorOpacityProperty =
        AvaloniaProperty.Register<AnimatedTextBox, double>(nameof(InternalErrorOpacity), 0.0);

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

    public static readonly StyledProperty<Brush> WatermarkForegroundProperty =
        AvaloniaProperty.Register<AnimatedTextBox, Brush>(nameof(WatermarkForeground));

    public Brush WatermarkForeground
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

    private char _originalPasswordChar = '\0';

    protected override void OnInitialized()
    {
        base.OnInitialized();
        
        // Sincroniza o estado inicial no carregamento para evitar saltos de layout
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

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == HasErrorProperty || change.Property == ErrorProperty)
        {
            UpdateErrorAnimation();
        }

        if (change.Property == IsActiveProperty)
        {
            UpdateAnimationState(change.GetNewValue<bool>());
        }
        
        if (change.Property == BoundsProperty)
        {
            // Update error height if size changes and it's visible
            if (HasError) UpdateErrorAnimation();
        }

        if (change.Property == RevealPasswordProperty)
        {
            var value = change.GetNewValue<bool>();
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

        if (change.Property == PasswordCharProperty && !RevealPassword)
        {
            _originalPasswordChar = change.GetNewValue<char>();
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

            // Ativação: Cresce e aparece junto (Opacidade levemente mais rápida)
            InternalMaxHeight = 150;
            await Task.Delay(300);
            InternalOpacity = 1;
        }
        else
        {
            // Desativação: Some conteúdo primeiro, depois fecha
            InternalOpacity = 0;
            
            IsEnabled = false;
            IsTabStop = false;
            Focusable = false;

            if (token != _stateToken) return;

            InternalMaxHeight = 0;
        }
    }

    private async void UpdateErrorAnimation()
    {
        var token = new TransitionToken();
        _errorStateToken = token;

        if (HasError && !string.IsNullOrEmpty(Error))
        {
            // If the text actually changed and it's already showing, fade out first
            if (InternalErrorText != Error && InternalErrorOpacity > 0)
            {
                InternalErrorOpacity = 0;
                await Task.Delay(200); // 0.2s duration from XAML double transition
                if (token != _errorStateToken) return;
            }

            InternalErrorText = Error;

            // Measure actual error text height
            var textBlock = new TextBlock
            {
                Text = Error,
                FontSize = 12,
                TextWrapping = TextWrapping.Wrap
            };
            
            var availableWidth = Math.Max(10, Bounds.Width - 5);
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