using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Arpg.Client.Views;

public partial class TemplateEditorView : UserControl
{
    public TemplateEditorView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
