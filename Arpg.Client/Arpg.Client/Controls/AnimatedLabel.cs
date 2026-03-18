using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Arpg.Client.Controls;

public class AnimatedLabel : TemplatedControl
{
    private Guid _stateToken;

    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<AnimatedLabel, string?>(nameof(Text));

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly StyledProperty<double> InternalOpacityProperty =
        AvaloniaProperty.Register<AnimatedLabel, double>(nameof(InternalOpacity), 0.6);

    public double InternalOpacity
    {
        get => GetValue(InternalOpacityProperty);
        set => SetValue(InternalOpacityProperty, value);
    }

    public static readonly StyledProperty<string?> DisplayTextProperty =
        AvaloniaProperty.Register<AnimatedLabel, string?>(nameof(DisplayText));

    public string? DisplayText
    {
        get => GetValue(DisplayTextProperty);
        set => SetValue(DisplayTextProperty, value);
    }

    public static readonly StyledProperty<double> LetterSpacingProperty =
        AvaloniaProperty.Register<AnimatedLabel, double>(nameof(LetterSpacing), 0.0);

    public double LetterSpacing
    {
        get => GetValue(LetterSpacingProperty);
        set => SetValue(LetterSpacingProperty, value);
    }

    static AnimatedLabel()
    {
        TextProperty.Changed.AddClassHandler<AnimatedLabel>((x, e) => x.OnTextChanged(e));
    }

    private async void OnTextChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var newText = e.GetNewValue<string?>();
        
        // Se for a primeira vez, apenas seta o texto
        if (DisplayText == null)
        {
            DisplayText = newText;
            return;
        }

        var token = Guid.NewGuid();
        _stateToken = token;

        // Fase 1: Fade Out
        InternalOpacity = 0;
        
        await Task.Delay(200); // Espera o fade out (ajustado para a duração da transição no XAML)
        if (token != _stateToken) return;

        // Fase 2: Troca o texto
        DisplayText = newText;

        // Fase 3: Fade In
        InternalOpacity = 0.6;
    }
}
