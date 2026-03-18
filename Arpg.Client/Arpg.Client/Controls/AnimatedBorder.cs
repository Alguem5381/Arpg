using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace Arpg.Client.Controls;

public class AnimatedBorder : ContentControl
{
    private TransitionToken _stateToken = new();

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
        AvaloniaProperty.Register<AnimatedBorder, double>(nameof(InternalMaxHeight), 0.0);

    public double InternalMaxHeight
    {
        get => GetValue(InternalMaxHeightProperty);
        set => SetValue(InternalMaxHeightProperty, value);
    }

    public static readonly StyledProperty<double> InternalOpacityProperty =
        AvaloniaProperty.Register<AnimatedBorder, double>(nameof(InternalOpacity), 0.0);

    public double InternalOpacity
    {
        get => GetValue(InternalOpacityProperty);
        set => SetValue(InternalOpacityProperty, value);
    }

    static AnimatedBorder()
    {
        AnimateProperty.Changed.AddClassHandler<AnimatedBorder>((x, e) => x.OnAnimateChanged(e));
    }

    private void OnAnimateChanged(AvaloniaPropertyChangedEventArgs e)
    {
        UpdateAnimationState(e.GetNewValue<bool>());
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

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        
        if (change.Property == ContentProperty && Animate)
        {
            InternalMaxHeight = CalculateTargetHeight();
        }
    }

    private double CalculateTargetHeight()
    {
        if (Content == null) return 0;

        if (Content is Layoutable l)
        {
            l.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            return l.DesiredSize.Height + TargetPadding.Top + TargetPadding.Bottom + 4;
        }

        return 100; // Fallback
    }

    private async void UpdateAnimationState(bool isActive)
    {
        var token = new TransitionToken();
        _stateToken = token;

        if (isActive)
        {
            double targetHeight = 150; 
            if (Content is Layoutable l)
            {
                l.Measure(new Size(Bounds.Width > 0 ? Bounds.Width : double.PositiveInfinity, double.PositiveInfinity));
                targetHeight = l.DesiredSize.Height + TargetPadding.Top + TargetPadding.Bottom + 4;
            }

            Padding = TargetPadding;
            InternalMaxHeight = targetHeight;
            await Task.Delay(300);
            if (token != _stateToken) return;
            InternalOpacity = 1;
        }
        else
        {
            InternalOpacity = 0;
            await Task.Delay(200);
            if (token != _stateToken) return;
            InternalMaxHeight = 0;
            Padding = new Thickness(0);
        }
    }

    private class TransitionToken
    {
        public Guid Id = Guid.NewGuid();
    }
}