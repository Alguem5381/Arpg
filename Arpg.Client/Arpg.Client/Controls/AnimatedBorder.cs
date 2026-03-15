using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;

namespace Arpg.Client.Controls;

[PseudoClasses(":animate")]
public class AnimatedBorder : Label
{
    public static readonly StyledProperty<Thickness> TargetPaddingProperty =
        AvaloniaProperty.Register<AnimatedBorder, Thickness>(nameof(TargetPadding));

    public Thickness TargetPadding
    {
        get => GetValue(TargetPaddingProperty);
        set => SetValue(TargetPaddingProperty, value);
    }
    
    public static readonly StyledProperty<bool> AnimateProperty =
        AvaloniaProperty.Register<AnimatedBorder, bool>(nameof(Animate));

    public bool Animate
    {
        get => GetValue(AnimateProperty);
        set => SetValue(AnimateProperty, value);
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
    
        if (change.Property == AnimateProperty)
        {
            PseudoClasses.Set(":animate", Animate);
        }
    }
}