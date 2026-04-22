using Arpg.Primitives.Codes;
using Arpg.Primitives.Constants;

namespace Arpg.Primitives.Results;

public class Error(string message)
{
    public string Message { get; protected set; } = message;
    public Dictionary<string, object> Metadata { get; protected set; } = [];

    public Error With(string key, object value)
    {
        Metadata[key] = value;
        return this;
    }

    public string GetCode()
    {
        Metadata.TryGetValue(Key.Error, out var obj);

        return obj as string ?? this switch
        {
            NotFoundError => GeneralCodes.NotFound,
            ConflictError => GeneralCodes.Conflict,
            _ => GeneralCodes.Generic
        };
    }
}
