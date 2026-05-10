using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Arpg.Client.Views;

public partial class MainView : UserControl
{
    // Define a largura em pixels em que o layout muda para "mobile"
    private const double MobileBreakPoint = 800.0;

    public MainView()
    {
        InitializeComponent();
    }

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);

        // Se a largura for menor ou igual ao BreakPoint, ativa o modo mobile
        if (e.NewSize.Width <= MobileBreakPoint)
        {
            if (AppSplitView.Classes.Contains("mobile")) return;

            AppSplitView.Classes.Add("mobile");
            // Ao entrar no modo mobile, o menu deve começar fechado
            AppSplitView.IsPaneOpen = false;
        }
        else // Modo desktop
        {
            if (!AppSplitView.Classes.Contains("mobile")) return;

            AppSplitView.Classes.Remove("mobile");
            // Ao voltar para o desktop, o menu volta a ficar aberto
            AppSplitView.IsPaneOpen = true;
        }
    }

    private void OnHamburgerButtonClick(object? sender, RoutedEventArgs e)
    {
        // Alterna o estado do painel (Abre se estiver fechado, fecha se estiver aberto)
        AppSplitView.IsPaneOpen = !AppSplitView.IsPaneOpen;
    }
}