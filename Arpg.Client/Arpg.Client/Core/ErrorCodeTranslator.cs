using System.Collections.Generic;
using System.Linq;

using Arpg.Shared.Codes;

namespace Arpg.Client.Core;

public static class ErrorTranslator
{
    private static readonly Dictionary<string, string> Messages = new()
    {
        { DataFormatCodes.InvalidSize, "É necessário no mínimo {MinLength} e no máximo {MaxLength} caractéres" },
        { UserCodes.InvalidCredentials, "Senha ou usuário incorretos" },
        { UserCodes.InvalidUsernameFormat, "Nome de usuário só pode conter letras, numeros e underscore"},
        { DataFormatCodes.Required, "Obrigatório"},
        { GeneralCodes.Domain, "Correu um erro"},
        { GeneralCodes.NoConnection, "Não foi possível se conectar" },
        { GeneralCodes.Timeout, "Servidor demorou responder"},
        { UserCodes.PasswordMismatch, "As senhas não são iguais" },
        { DataFormatCodes.InvalidEmailFormat, "Email inválido"}
    };

    public static string ToMessage(ApiError? error)
    {
        if (error == null)
            return "Error";
        
        if (!Messages.TryGetValue(error.Code, out var template))
            return error.Message;

        try
        {
            return error.Metadata.Aggregate(template, (currentTemplate, item) => currentTemplate.Replace($"{{{item.Key}}}", item.Value.ToString()));
        }
        catch
        {
            return error.Message;
        }
    }
}