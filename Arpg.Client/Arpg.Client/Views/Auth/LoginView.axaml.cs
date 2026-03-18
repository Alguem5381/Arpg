using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Threading;
using Arpg.Client.ViewModels.Auth;

namespace Arpg.Client.Views.Auth;

public partial class LoginView : UserControl
{
    private Border? _card;
    private Border? _separator;
    private TextBlock? _title;
    private Avalonia.Controls.Control? _subtitle;
    private StackPanel? _fields;
    private StackPanel? _actions;
    private ScrollViewer? _scrollViewer;
    private Rectangle? _keyboardSpacer;

    public LoginView()
    {
        InitializeComponent();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        _card      = this.FindControl<Border>("PART_Card");
        _separator = this.FindControl<Border>("PART_Separator");
        _title     = this.FindControl<TextBlock>("PART_Title");
        _subtitle  = this.FindControl<Control>("PART_Subtitle");
        _fields    = this.FindControl<StackPanel>("PART_Fields");
        _actions   = this.FindControl<StackPanel>("PART_Actions");
        _scrollViewer = this.FindControl<ScrollViewer>("PART_ScrollViewer");
        _keyboardSpacer = this.FindControl<Rectangle>("PART_KeyboardSpacer");

        // Gerenciamento de teclado (occluded rect) para Android/iOS
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
                    // Pequeno delay para garantir que o layout atualizou o spacer
                    Dispatcher.UIThread.Post(() => {
                         var focused = (topLevel as TopLevel)?.FocusManager?.GetFocusedElement() as Avalonia.Controls.Control;
                         focused?.BringIntoView();
                    }, DispatcherPriority.Background);
                }
            };
        }

        // Hook de navegação: intercepta para executar a animação de saída primeiro
        if (DataContext is LoginViewModel vm)
        {
            var originalNavigate = vm.NavigateToMainPage;
            vm.NavigateToMainPage = async () =>
            {
                await PlayExitAnimationAsync();
                originalNavigate?.Invoke();
            };
        }

        PlayEntranceAnimation();
    }

    private void PlayEntranceAnimation()
    {
        // Garante estado inicial antes das transições começarem
        SetInitialState();

        Dispatcher.UIThread.Post(async () =>
        {
            await Task.Delay(50); // espera o render inicial

            // Fase 1: Card cresce horizontalmente e aparece
            if (_card != null)
            {
                _card.RenderTransform = null; // scaleX(1) scaleY(1) — estado final
                _card.Opacity = 1;
            }

            await Task.Delay(330); // aguarda o card expandir

            // Fase 2: Separador azul cresce horizontalmente
            if (_separator != null)
            {
                _separator.MaxWidth = double.PositiveInfinity;
                _separator.Opacity = 1;
            }

            await Task.Delay(320);

            // Fase 3: Título ARPG aparece
            if (_title != null)
                _title.Opacity = 1;

            await Task.Delay(200);

            // Fase 4: Subtítulo
            if (_subtitle != null)
                _subtitle.Opacity = 1;

            await Task.Delay(150);

            // Fase 5: Campos do formulário
            if (_fields != null)
                _fields.Opacity = 1;

            await Task.Delay(150);

            // Fase 6: Botões de ação
            if (_actions != null)
                _actions.Opacity = 1;

        }, DispatcherPriority.Loaded);
    }

    private async Task PlayExitAnimationAsync()
    {
        // Saída reversa, mais rápida que a entrada
        if (_actions != null) _actions.Opacity = 0;
        await Task.Delay(100);

        if (_fields != null)  _fields.Opacity = 0;
        if (_subtitle != null) _subtitle.Opacity = 0;
        await Task.Delay(120);

        if (_title != null)     _title.Opacity = 0;
        if (_separator != null) { _separator.MaxWidth = 0; _separator.Opacity = 0; }
        await Task.Delay(200);

        // Card sai com scale e fade
        if (_card != null)
        {
            _card.RenderTransform = Avalonia.Media.Transformation.TransformOperations.Parse("scale(1.05)");
            _card.Opacity = 0;
        }

        await Task.Delay(420); // espera a animação do card terminar
    }

    private void SetInitialState()
    {
        if (_card != null)
        {
            _card.Opacity = 0;
            _card.RenderTransform = Avalonia.Media.Transformation.TransformOperations.Parse("scaleX(0.1) scaleY(1)");
        }

        if (_separator != null)
        {
            _separator.MaxWidth = 0;
            _separator.Opacity = 0;
        }

        if (_title != null)    _title.Opacity = 0;
        if (_subtitle != null) _subtitle.Opacity = 0;
        if (_fields != null)   _fields.Opacity = 0;
        if (_actions != null)  _actions.Opacity = 0;
    }
}