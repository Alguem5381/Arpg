using System.Collections.Generic;
using System.Linq;

using Arpg.Primitives.Codes;

namespace Arpg.Client.Core;

public static class ErrorTranslator
{
    private static readonly Dictionary<string, string> Messages = new()
    {
        { DataFormatCodes.InvalidSize, "É necessário no mínimo {MinLength} e no máximo {MaxLength} caracteres" },
        { DataFormatCodes.InvalidUniqueSize, "O campo deve ter exatamente {MaxLength} dígitos" },
        { UserCodes.InvalidCredentials, "Senha ou usuário incorretos" },
        { UserCodes.InvalidUsernameFormat, "Nome de usuário só pode conter letras, numeros e underscore"},
        { DataFormatCodes.Required, "Obrigatório"},
        { GeneralCodes.Domain, "Ocorreu um erro"},
        { GeneralCodes.NoConnection, "Não foi possível se conectar" },
        { GeneralCodes.Timeout, "Servidor demorou responder"},
        { UserCodes.PasswordMismatch, "As senhas não são iguais" },
        { DataFormatCodes.InvalidEmail, "Email inválido"},
        { UserCodes.UserConflict, "Usuário já está em uso" },
        { CodeCodes.CodeNotFound, "Código não encontrado ou já utilizado" },
        { CodeCodes.InvalidCode, "Código de verificação incorreto" },
        { CodeCodes.CodeExpired, "O código expirou, solicite um novo" }
    };

    public static string ToMessage(ApiError? error)
    {
        if (error == null)
            return "Error";

        if (!Messages.TryGetValue(error.Code, out var template))
            return error.Message;

        try
        {
            return (error.Metadata ?? []).Aggregate(template, (currentTemplate, item) =>
                currentTemplate.Replace($"{{{item.Key}}}", item.Value.ToString() ?? string.Empty, System.StringComparison.OrdinalIgnoreCase));
        }
        catch
        {
            return error.Message;
        }
    }
}