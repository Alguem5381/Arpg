using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Arpg.Client.Controls;

public class AnimatedButton : Button
{
    private Guid _stateToken;

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
        ContentProperty.Changed.AddClassHandler<AnimatedButton>((x, e) => x.OnContentChanged(e));
    }

    private async void OnContentChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var newContent = e.GetNewValue<object?>();
        
        if (DisplayContent == null)
        {
            DisplayContent = newContent;
            return;
        }

        var token = Guid.NewGuid();
        _stateToken = token;

        // Fase 1: Fade Out
        InternalOpacity = 0;
        
        await Task.Delay(150);
        if (token != _stateToken) return;

        // Medição do novo conteúdo usando um TextBlock temporário
        var textBlock = new TextBlock
        {
            Text = newContent?.ToString() ?? "",
            FontSize = FontSize,
            FontWeight = FontWeight,
            FontFamily = FontFamily,
            FontStyle = FontStyle
        };
        textBlock.Measure(Size.Infinity);
        
        var targetWidth = textBlock.DesiredSize.Width + Padding.Left + Padding.Right + BorderThickness.Left + BorderThickness.Right;
        var oldWidth = Bounds.Width;

        if (Math.Abs(oldWidth - targetWidth) > 1)
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
                        Setters = { new Avalonia.Styling.Setter(WidthProperty, oldWidth) }
                    },
                    new Avalonia.Animation.KeyFrame
                    {
                        Cue = new Avalonia.Animation.Cue(1d),
                        Setters = { new Avalonia.Styling.Setter(WidthProperty, targetWidth) }
                    }
                }
            };

            // Inicia o fade in do texto com um atraso maior para sincronizar com o redimensionamento
            _ = Task.Run(async () =>
            {
                await Task.Delay(250);
                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                {
                    if (token == _stateToken) InternalOpacity = 1;
                });
            });

            await animation.RunAsync(this);
            Width = targetWidth; // Garante o valor final
        }
        else
        {
            DisplayContent = newContent;
            InternalOpacity = 1;
        }

        // Libera a largura para Auto após a transição terminar
        if (token == _stateToken) Width = double.NaN;
    }
}