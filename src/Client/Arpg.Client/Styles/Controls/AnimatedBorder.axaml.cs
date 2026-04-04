using System.Threading.Tasks;
using Arpg.Client.Controls.Common;
using Arpg.Client.Extensions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace Arpg.Client.Styles.Controls;

public class AnimatedBorder : ContentControl
{
    private TransitionToken _stateToken = TransitionToken.Create();

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

    public static readonly StyledProperty<double> InternalMaxHeightProperty =
        AvaloniaProperty.Register<AnimatedBorder, double>(nameof(InternalMaxHeight));

    public double InternalMaxHeight
    {
        get => GetValue(InternalMaxHeightProperty);
        set => SetValue(InternalMaxHeightProperty, value);
    }

    public static readonly StyledProperty<double> InternalOpacityProperty =
        AvaloniaProperty.Register<AnimatedBorder, double>(nameof(InternalOpacity));

    public double InternalOpacity
    {
        get => GetValue(InternalOpacityProperty);
        set => SetValue(InternalOpacityProperty, value);
    }

    static AnimatedBorder()
    {
        AnimateProperty.Changed.AddClassHandler<AnimatedBorder>((x, e) => x.OnAnimateChanged(e));
        ContentProperty.Changed.AddClassHandler<AnimatedBorder>((x, _) => x.OnContentPropertyChanged());
        BoundsProperty.Changed.AddClassHandler<AnimatedBorder>((x, _) => x.OnBoundsChanged());
    }

    private void OnAnimateChanged(AvaloniaPropertyChangedEventArgs e)
        => _ = UpdateAnimationState(e.GetNewValue<bool>());

    private void OnContentPropertyChanged()
    {
        if (Animate)
            InternalMaxHeight = CalculateTargetHeight();
    }

    private void OnBoundsChanged()
    {
        if (Animate)
            InternalMaxHeight = CalculateTargetHeight();
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (!Animate)
        {
            InternalMaxHeight = 0;
            InternalOpacity = 0;
            Padding = new Thickness(0);
        }
        else
        {
            InternalMaxHeight = CalculateTargetHeight();
            InternalOpacity = 1;
            Padding = TargetPadding;
        }
    }

    private double CalculateTargetHeight()
    {
        if (Content is Layoutable layout)
            return layout.MeasureContentHeight(double.PositiveInfinity, TargetPadding) + 4;

        return Content == null ? 0 : 100;
    }

    private async Task UpdateAnimationState(bool isActive)
    {
        var token = TransitionToken.Create();
        _stateToken = token;

        if (isActive)
        {
            double targetHeight = 150;

            if (Content is Layoutable layout)
            {
                var availableWidth = Bounds.Width > 0 ? Bounds.Width : double.PositiveInfinity;
                targetHeight = layout.MeasureContentHeight(availableWidth, TargetPadding) + 4;
            }

            Padding = TargetPadding;
            InternalMaxHeight = targetHeight;

            await Task.Delay(300);

            if (_stateToken != token) return;
            InternalOpacity = 1;
        }
        else
        {
            InternalOpacity = 0;

            await Task.Delay(200);

            if (_stateToken != token) return;

            InternalMaxHeight = 0;
            Padding = new Thickness(0);
        }
    }
}