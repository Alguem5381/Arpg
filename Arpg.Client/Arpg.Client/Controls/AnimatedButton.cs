using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Arpg.Client.Controls;

public class AnimatedButton : Button
{
    public static readonly StyledProperty<Brush?> BackgroundOnHoverProperty = 
        AvaloniaProperty.Register<AnimatedButton, Brush?>(nameof(BackgroundOnHover));

    public Brush? BackgroundOnHover
    {
        get => GetValue(BackgroundOnHoverProperty);
        set => SetValue(BackgroundOnHoverProperty, value);
    }
}