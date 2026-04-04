using System;
using System.Threading.Tasks;
using Arpg.Client.Controls.Common;
using Arpg.Client.Extensions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;

namespace Arpg.Client.Styles.Controls;

public class AnimatedButton : Button
{
    private TransitionToken _stateToken = TransitionToken.Create();

    public static readonly StyledProperty<Brush?> BackgroundOnHoverProperty = 
        AvaloniaProperty.Register<AnimatedButton, Brush?>(nameof(BackgroundOnHover));

    public Brush? BackgroundOnHover
    {
        get => GetValue(BackgroundOnHoverProperty);
        set => SetValue(BackgroundOnHoverProperty, value);
    }

    public static readonly StyledProperty<double> InternalOpacityProperty =
        AvaloniaProperty.Register<AnimatedButton, double>(nameof(InternalOpacity), 1.0);

    public double InternalOpacity
    {
        get => GetValue(InternalOpacityProperty);
        set => SetValue(InternalOpacityProperty, value);
    }

    public static readonly StyledProperty<object?> DisplayContentProperty =
        AvaloniaProperty.Register<AnimatedButton, object?>(nameof(DisplayContent));

    public object? DisplayContent
    {
        get => GetValue(DisplayContentProperty);
        set => SetValue(DisplayContentProperty, value);
    }

    static AnimatedButton()
    {
        ContentProperty.Changed.AddClassHandler<AnimatedButton>((x, e) => _ = x.OnContentChanged(e));
    }

    private async Task OnContentChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var newContent = e.GetNewValue<object?>();
        
        if (DisplayContent == null)
        {
            DisplayContent = newContent;
            return;
        }

        var token = TransitionToken.Create();
        _stateToken = token;

        InternalOpacity = 0;
        
        await Task.Delay(150);
        if (_stateToken != token) return;
        
        var desiredWidth = Visual.MeasureTextWidth(newContent?.ToString() ?? "",
            FontSize,
            FontWeight,
            FontFamily,
            FontStyle);
        
        var realTargetWidth = desiredWidth + Padding.Left + Padding.Right + BorderThickness.Left + BorderThickness.Right;
        var oldWidth = Bounds.Width;

        if (Math.Abs(oldWidth - realTargetWidth) > 1)
        {
            Width = oldWidth;
            DisplayContent = newContent;

            var animation = new Avalonia.Animation.Animation
            {
                Duration = TimeSpan.FromMilliseconds(400),
                Easing = new Avalonia.Animation.Easings.CubicEaseInOut(),
                FillMode = Avalonia.Animation.FillMode.Forward,
                Children =
                {
                    new Avalonia.Animation.KeyFrame
                    {
                        Cue = new Avalonia.Animation.Cue(0d),
                        Setters = { new Setter(WidthProperty, oldWidth) }
                    },
                    new Avalonia.Animation.KeyFrame
                    {
                        Cue = new Avalonia.Animation.Cue(1d),
                        Setters = { new Setter(WidthProperty, realTargetWidth) }
                    }
                }
            };

            // Inicia o fade in do texto com um atraso maior para sincronizar com o redimensionamento
            _ = Task.Run(async () =>
            {
                await Task.Delay(250);
                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                {
                    if (_stateToken == token) InternalOpacity = 1;
                });
            });

            await animation.RunAsync(this);
            Width = realTargetWidth; // Garante o valor final
        }
        else
        {
            DisplayContent = newContent;
            InternalOpacity = 1;
        }

        // Libera a largura para Auto após a transição terminar
        if (_stateToken == token) Width = double.NaN;
    }
}