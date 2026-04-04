using System.Threading.Tasks;
using Arpg.Client.Controls.Common;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace Arpg.Client.Styles.Controls;

public class AnimatedLabel : TemplatedControl
{
    private TransitionToken _stateToken = TransitionToken.Create();

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

    public new static readonly StyledProperty<double> LetterSpacingProperty =
        AvaloniaProperty.Register<AnimatedLabel, double>(nameof(LetterSpacing));

    public new double LetterSpacing
    {
        get => GetValue(LetterSpacingProperty);
        set => SetValue(LetterSpacingProperty, value);
    }

    static AnimatedLabel()
    {
        TextProperty.Changed.AddClassHandler<AnimatedLabel>((x, e) => _ = x.OnTextChanged(e));
    }

    private async Task OnTextChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var newText = e.GetNewValue<string?>();

        if (DisplayText == null)
        {
            DisplayText = newText;
            return;
        }

        var token = TransitionToken.Create();
        _stateToken = token;

        InternalOpacity = 0;

        await Task.Delay(200);
        if (_stateToken != token) return;

        DisplayText = newText;

        InternalOpacity = 0.6;
    }
}
