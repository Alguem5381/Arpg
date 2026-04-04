using Arpg.Client.ViewModels.Auth;
using Avalonia.Data.Converters;

namespace Arpg.Client.Views.Auth;

public static class AuthViewModeConverters
{
    public static readonly FuncValueConverter<AuthViewMode, string> SubmitButtonText = new(mode => mode switch
    {
        AuthViewMode.Login => "Entrar",
        AuthViewMode.Register => "Registrar",
        AuthViewMode.Validate => "Validar",
        _ => "Continuar"
    });

    public static readonly FuncValueConverter<AuthViewMode, string> ToggleButtonText = new(mode => mode switch
    {
        AuthViewMode.Login => "Criar conta",
        AuthViewMode.Register => "Voltar",
        AuthViewMode.Validate => "Voltar ao Login",
        _ => "Voltar"
    });

    public static readonly FuncValueConverter<AuthViewMode, string> Subtitle = new(mode => mode switch
    {
        AuthViewMode.Login => "ENTRAR",
        AuthViewMode.Register => "CRIAR CONTA",
        AuthViewMode.Validate => "VALIDAR CÓDIGO",
        _ => ""
    });

    public static readonly FuncValueConverter<AuthViewMode, string> NotificationText = new(mode =>
        mode == AuthViewMode.Validate
            ? "Um código de validação foi enviado para o seu e-mail."
            : string.Empty);
}
