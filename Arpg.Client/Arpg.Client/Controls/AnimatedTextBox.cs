using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;
using Avalonia.Media;

namespace Arpg.Client.Controls;

[PseudoClasses(":has-error")]
public class AnimatedTextBox : TextBox
{
    private const string HasErrorClassName = "has-error";
    
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
    
    public  static readonly StyledProperty<Brush> OutsideBorderBrushProperty  =
        AvaloniaProperty.Register<AnimatedTextBox, Brush>(nameof(OutsideBorderBrush));

    public Brush OutsideBorderBrush
    {
        get => GetValue(OutsideBorderBrushProperty);
        set => SetValue(OutsideBorderBrushProperty, value);
    }
    
    public  static readonly StyledProperty<CornerRadius> OutsideCornerRadiusProperty  =
        AvaloniaProperty.Register<AnimatedTextBox, CornerRadius>(nameof(OutsideCornerRadius));
    
    public CornerRadius OutsideCornerRadius
    {
        get => GetValue(OutsideCornerRadiusProperty);
        set => SetValue(OutsideCornerRadiusProperty, value);
    }
    
    public  static readonly StyledProperty<Thickness> OutsideBorderThicknessProperty  =
        AvaloniaProperty.Register<AnimatedTextBox, Thickness>(nameof(OutsideBorderThickness));

    public Thickness OutsideBorderThickness
    {
        get => GetValue(OutsideBorderThicknessProperty);
        set => SetValue(OutsideBorderThicknessProperty, value);
    }
    
    public  static readonly StyledProperty<Thickness> OutsidePaddingProperty  =
        AvaloniaProperty.Register<AnimatedTextBox, Thickness>(nameof(OutsidePadding));

    public Thickness OutsidePadding
    {
        get => GetValue(OutsidePaddingProperty);
        set => SetValue(OutsidePaddingProperty, value);
    }
    
    public  static readonly StyledProperty<Brush> OutsideBackgroundProperty  =
        AvaloniaProperty.Register<AnimatedTextBox, Brush>(nameof(OutsideBackground));
    
    public Brush OutsideBackground
    {
        get => GetValue(OutsideBackgroundProperty);
        set => SetValue(OutsideBackgroundProperty, value);
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
    
        if (change.Property == HasErrorProperty)
        {
            PseudoClasses.Set(":has-error", HasError);
        }
    }
}