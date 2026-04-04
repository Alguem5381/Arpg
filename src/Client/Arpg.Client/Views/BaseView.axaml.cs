using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Threading;

namespace Arpg.Client.Views;

public partial class BaseView : UserControl
{
    private Rectangle? _keyboardSpacer;

    public BaseView()
    {
        InitializeComponent();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        _keyboardSpacer = this.FindControl<Rectangle>("PART_KeyboardSpacer");

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel?.InputPane != null)
        {
            topLevel.InputPane.StateChanged += (s, args) =>
            {
                var rect = topLevel.InputPane.OccludedRect;

                if (_keyboardSpacer != null)
                    _keyboardSpacer.Height = rect.Height;

                if (rect.Height > 0)
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        var focused = topLevel.FocusManager?.GetFocusedElement() as Control;
                        focused?.BringIntoView();
                    }, DispatcherPriority.Background);
                }
            };
        }
    }
}
