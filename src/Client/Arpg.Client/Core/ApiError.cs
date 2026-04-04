using System.Collections.Generic;

using FluentResults;

namespace Arpg.Client.Core;

public class ApiError(string message, string code, Dictionary<string, object> metadata) : IError
{
    public string Message { get; } = message;
    public Dictionary<string, object> Metadata { get; } = metadata;
    public List<IError> Reasons { get; } = [];
    public string Code { get; } = code;

    public void WithMetadata(string key, object value)
    {
        Metadata[key] = value;
    }
}