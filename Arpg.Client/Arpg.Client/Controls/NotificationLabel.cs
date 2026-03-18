using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace Arpg.Client.Controls;

public class NotificationLabel : TemplatedControl
{
    private Guid _stateToken;

    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<NotificationLabel, string?>(nameof(Text));

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly StyledProperty<double> InternalOpacityProperty =
        AvaloniaProperty.Register<NotificationLabel, double>(nameof(InternalOpacity), 0.0);

    public double InternalOpacity
    {
        get => GetValue(InternalOpacityProperty);
        set => SetValue(InternalOpacityProperty, value);
    }

    public static readonly StyledProperty<double> InternalMaxHeightProperty =
        AvaloniaProperty.Register<NotificationLabel, double>(nameof(InternalMaxHeight), 0.0);

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
        TextProperty.Changed.AddClassHandler<NotificationLabel>((x, e) => x.OnTextChanged(e));
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        // Se a largura mudar enquanto temos texto, recalculamos o MaxHeight para evitar cortes
        if (change.Property == BoundsProperty && !string.IsNullOrEmpty(Text))
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

        // Se Bounds.Width ainda for 0, usamos um padrão seguro. 
        // Mas como agora ouvimos a mudança de Bounds, ele recalculará assim que o layout ocorrer.
        var availableWidth = Bounds.Width > 0 ? Bounds.Width : 250;

        var textBlock = new TextBlock
        {
            Text = newText,
            FontSize = FontSize,
            FontWeight = FontWeight,
            TextWrapping = TextWrapping.Wrap,
            TextAlignment = TextAlignment.Center
        };
        
        // Medimos o TextBlock com a largura real disponível
        textBlock.Measure(new Size(availableWidth, double.PositiveInfinity));
        
        // A MaxHeight interna deve ser EXATAMENTE a altura do texto. 
        // Não somamos as margens aqui porque elas são aplicadas por fora do componente (no controle em si).
        InternalMaxHeight = textBlock.DesiredSize.Height;
    }

    private async void OnTextChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var newText = e.GetNewValue<string?>();
        var token = Guid.NewGuid();
        _stateToken = token;

        if (string.IsNullOrEmpty(newText))
        {
            // Fase de desaparecimento
            InternalOpacity = 0;
            Margin = new Thickness(0);
            RecalculateHeight(null);
            
            await Task.Delay(300);
            if (token != _stateToken) return;
            DisplayText = string.Empty;
        }
        else
        {
            // Fase de aparecimento
            DisplayText = newText;
            
            // Recalcula o espaço necessário
            RecalculateHeight(newText);
            
            // Aplica a margem real no controle para o StackPanel respeitar
            Margin = TargetMargin;
            
            await Task.Delay(100);
            if (token != _stateToken) return;

            InternalOpacity = 0.6;
        }
    }
}
