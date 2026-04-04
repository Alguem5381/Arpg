using System.Threading.Tasks;
using Arpg.Client.Controls.Common;
using Arpg.Client.Extensions;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace Arpg.Client.Styles.Controls;

public class NotificationLabel : TemplatedControl
{
    private TransitionToken _stateToken = TransitionToken.Create();

    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<NotificationLabel, string?>(nameof(Text));

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly StyledProperty<double> InternalOpacityProperty =
        AvaloniaProperty.Register<NotificationLabel, double>(nameof(InternalOpacity));

    public double InternalOpacity
    {
        get => GetValue(InternalOpacityProperty);
        set => SetValue(InternalOpacityProperty, value);
    }

    public static readonly StyledProperty<double> InternalMaxHeightProperty =
        AvaloniaProperty.Register<NotificationLabel, double>(nameof(InternalMaxHeight));

    public double InternalMaxHeight
    {
        get => GetValue(InternalMaxHeightProperty);
        set => SetValue(InternalMaxHeightProperty, value);
    }

    public static readonly StyledProperty<string?> DisplayTextProperty =
        AvaloniaProperty.Register<NotificationLabel, string?>(nameof(DisplayText));

    public string? DisplayText
    {
        get => GetValue(DisplayTextProperty);
        set => SetValue(DisplayTextProperty, value);
    }

    public static readonly StyledProperty<Thickness> TargetMarginProperty =
        AvaloniaProperty.Register<NotificationLabel, Thickness>(nameof(TargetMargin));

    public Thickness TargetMargin
    {
        get => GetValue(TargetMarginProperty);
        set => SetValue(TargetMarginProperty, value);
    }

    static NotificationLabel()
    {
        TextProperty.Changed.AddClassHandler<NotificationLabel>((x, e) => _ = x.OnTextChanged(e));
        BoundsProperty.Changed.AddClassHandler<NotificationLabel>((x, _) => x.OnBoundsChanged());
    }

    private void OnBoundsChanged()
    {
        if (!string.IsNullOrEmpty(Text))
        {
            RecalculateHeight(Text);
        }
    }



    private void RecalculateHeight(string? newText)
    {
        if (string.IsNullOrEmpty(newText))
        {
            InternalMaxHeight = 0;
            return;
        }

        var availableWidth = Bounds.Width > 0 ? Bounds.Width : 250;
        InternalMaxHeight = Visual.MeasureTextHeight(newText, availableWidth, FontSize, FontWeight);
    }

    private async Task OnTextChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var newText = e.GetNewValue<string?>();
        var token = TransitionToken.Create();
        _stateToken = token;

        if (string.IsNullOrEmpty(newText))
        {
            InternalOpacity = 0;
            Margin = new Thickness(0);
            RecalculateHeight(null);

            await Task.Delay(300);
            if (_stateToken != token) return;
            DisplayText = string.Empty;
        }
        else
        {
            DisplayText = newText;

            RecalculateHeight(newText);

            Margin = TargetMargin;

            await Task.Delay(100);
            if (_stateToken != token) return;

            InternalOpacity = 0.6;
        }
    }
}
